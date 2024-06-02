using MeetingScheduler.Contracts;
using MeetingScheduler.Data;
using MeetingScheduler.Entities;
using MeetingScheduler.Extensions;
using MeetingScheduler.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Endpoints;

public static class MeetingEndpoints
{
    public static WebApplication MapMeetingEndpoints(this WebApplication app)
    {
        app.MapPost("/api/meetings", async (AppDbContext dbContext, ICurrentUserProvider currentUserProvider, [FromBody] MeetingFilterPayload payload) =>
        {
            var currentUser = currentUserProvider.GetCurrentUserId();

            var query = await (from m in dbContext.Meetings
                join iu in dbContext.AppUsers on m.InvitedUserId equals iu.Id
                join cu in dbContext.AppUsers on m.CreatorId equals cu.Id
               join iutj in dbContext.UserTimeZones on m.InvitedUserId equals iutj.UserId into iutjGroup
                from iutj in iutjGroup.DefaultIfEmpty()
                where (payload.Title == null || m.Title.Contains(payload.Title)) &&
                      (payload.Description == null || m.Description.Contains(payload.Description)) &&
                      (payload.StartDateTime == null || m.StartDateTime >= payload.StartDateTime.Value.TryParseUtc()) &&
                      (m.CreatorId == currentUser || m.InvitedUserId == currentUser)
                select new
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    DateTime = m.StartDateTime,
                    Invited = iu.UserName,
                    InvitedId = iu.Id,
                    CreatorId = cu.Id,
                    Creator = cu.UserName,
                    InvitedUserTimeZone = iutj.TimeZone
                }).ToListAsync();


            return query.Select(x => new
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Creator = x.Creator,
                Invited = x.Invited,
                x.InvitedUserTimeZone,
                CreatorDateTime = currentUserProvider.ConvertDateTimeToUserTimeZone(x.CreatorId,x.DateTime),
                InvitedDateTime = currentUserProvider.ConvertDateTimeToUserTimeZone(x.InvitedId, x.DateTime)
            });
        }).WithTags("Meetings").RequireAuthorization();

        app.MapPost("/api/meetings/Create", async (AppDbContext dbContext, ICurrentUserProvider currentUserProvider, [FromBody] MeetingPayload meetingPayload) =>
        {
            var meeting = new Meeting
            {
                Title = meetingPayload.Title,
                Description = meetingPayload.Description,
                StartDateTime = meetingPayload.StartDateTime.TryParseUtc(),
                CreatorId = currentUserProvider.GetCurrentUserId(),
                InvitedUserId = meetingPayload.InvitedUserId
            };
            dbContext.Meetings.Add(meeting);
            await dbContext.SaveChangesAsync();
            return Results.Created($"/api/meetings/{meeting.Id}", meeting);
        }).WithTags("Meetings").RequireAuthorization();

        app.MapPut("/api/meetings/{id}", async (AppDbContext dbContext, long id, [FromBody] MeetingPayload meeting) =>
        {
            var existingMeeting = await dbContext.Meetings.FindAsync(id);
            if (existingMeeting == null)
            {
                return Results.NotFound();
            }

            existingMeeting.Title = meeting.Title;
            existingMeeting.Description = meeting.Description;
            existingMeeting.StartDateTime = meeting.StartDateTime.TryParseUtc();
            existingMeeting.InvitedUserId = meeting.InvitedUserId;

            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }).WithTags("Meetings").RequireAuthorization();

        app.MapDelete("/api/meetings/{id}", async (AppDbContext dbContext, long id) =>
        {
            var existingMeeting = await dbContext.Meetings.FindAsync(id);
            if (existingMeeting == null)
            {
                return Results.NotFound();
            }

            dbContext.Meetings.Remove(existingMeeting);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }).WithTags("Meetings").RequireAuthorization();

        return app;
    }
}