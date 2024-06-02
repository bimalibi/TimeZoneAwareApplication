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

        app.MapGet("/api/users", async (AppDbContext dbContext, ICurrentUserProvider currentUserProvider) =>
        {
            var currentUserId = currentUserProvider.GetCurrentUserId();
            return await dbContext.AppUsers
                .Where(x => x.Id != currentUserId)
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