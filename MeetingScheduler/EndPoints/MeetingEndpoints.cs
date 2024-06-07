using MeetingScheduler.Contracts;
using MeetingScheduler.Data;
using MeetingScheduler.Entities;
using MeetingScheduler.Extensions;
using MeetingScheduler.Helper;
using MeetingScheduler.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Endpoints;

public static class MeetingEndpoints
{
    public static WebApplication MapMeetingEndpoints(this WebApplication app)
    {
        app.MapPost("/api/meetings", async (AppDbContext dbContext, ICurrentUserProvider currentUserProvider, IDateHelper dateHelper, [FromBody] MeetingFilterPayload payload) =>
        {
            var currentUserId = currentUserProvider.GetCurrentUserId();
            var startDate = dateHelper.ToUtc(payload.StartDateTime);

            return await (from m in dbContext.Meetings
                join iu in dbContext.AppUsers on m.InvitedUserId equals iu.Id
                join cu in dbContext.AppUsers on m.CreatorId equals cu.Id
                join iutj in dbContext.UserTimeZones on m.InvitedUserId equals iutj.UserId
                where (payload.Title == null || m.Title.Contains(payload.Title)) &&
                      (payload.Description == null || m.Description.Contains(payload.Description)) &&
                      (startDate == null || m.StartDateTime.Date == startDate.Value.Date) &&
                      (m.CreatorId == currentUserId || m.InvitedUserId == currentUserId)
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
                    InvitedUserTimeZone = iutj.TimeZone,
                    CreatorDateTime = dateHelper.ToLocal(m.CreatorId, m.StartDateTime),
                    InvitedDateTime = dateHelper.ToLocal(m.InvitedUserId, m.StartDateTime)
                }).ToListAsync();
        }).WithTags("Meetings").RequireAuthorization();

        app.MapPost("/api/meetings/Create", async (AppDbContext dbContext, ICurrentUserProvider currentUserProvider, DateHelper dateHelper, [FromBody] MeetingPayload meetingPayload) =>
        {
            var startDate = dateHelper.ToUtc(meetingPayload.StartDateTime);
            var meeting = new Meeting
            {
                Title = meetingPayload.Title,
                Description = meetingPayload.Description,
                StartDateTime = startDate!.Value,
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