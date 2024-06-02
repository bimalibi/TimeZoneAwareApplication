using MeetingScheduler.Contracts;
using MeetingScheduler.Data;
using MeetingScheduler.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Endpoints;

public static class UserEndpoints
{
    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/user/me", (ICurrentUserProvider currentUserProvider) =>
        {
            var userId = currentUserProvider.GetCurrentUserId();
            var userTimeZone = currentUserProvider.UserTimeZone();
            return Results.Ok(new { userId, userTimeZone });
        }).WithTags("User").RequireAuthorization();

        app.MapPut("/api/users", async (AppDbContext dbContext, [FromForm] UserTimeZoneUpdatePayload payload) =>
        {
            return await dbContext.AppUsers
                .Where(x => x.Id == payload.UserId)
                .Select(x => new
                {
                    x.Id,
                    x.Email,
                    x.UserName
                })
                .ToListAsync();
        }).WithTags("User").RequireAuthorization();

        app.MapGet("/api/users", async (AppDbContext dbContext) =>
        {
            return await dbContext.AppUsers
                .Select(x => new
                {
                    x.Id,
                    x.Email,
                    x.UserName
                })
                .ToListAsync();
        }).WithTags("User").RequireAuthorization();
        return app;
    }
}