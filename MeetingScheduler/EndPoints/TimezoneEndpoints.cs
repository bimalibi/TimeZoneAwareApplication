
using MeetingScheduler.Contracts;
using MeetingScheduler.Data;
using MeetingScheduler.Entities;
using MeetingScheduler.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Endpoints;

public static class TimezoneEndpoints
{
    public static WebApplication MapTimezoneEndpoints(this WebApplication app)
    {
        app.MapGet("/api/timezones", () =>
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            return timeZones.Select(tz => tz);
        }).WithTags("Timezones").RequireAuthorization();

        app.MapPost("/api/timezone", async (AppDbContext dbContext, ICurrentUserProvider currentUserProvider, [FromBody] UserTimeZoneUpdatePayload payload) =>
        {
            var userId = currentUserProvider.GetCurrentUserId();
            await dbContext.UserTimeZones.Where(ut => ut.UserId == userId).ExecuteDeleteAsync();

            var userTimeZone = new UserTimeZone
            {
                UserId = userId,
                TimeZone = payload.TimeZone
            };
            dbContext.UserTimeZones.Add(userTimeZone);
            await dbContext.SaveChangesAsync();
            return Results.Created($"/api/timezones/{userTimeZone.Id}", userTimeZone);
        }).WithTags("Timezones").RequireAuthorization();
        
        app.MapGet("/api/timezone/{userId}", async (AppDbContext dbContext, ICurrentUserProvider currentUserProvider) =>
        {
            var userId = currentUserProvider.GetCurrentUserId();
            var userTimeZone = await dbContext.UserTimeZones
                .FirstOrDefaultAsync(ut => ut.UserId == userId);
            return userTimeZone;
        }).WithTags("Timezones").RequireAuthorization();
        
        return app;
    }
}
