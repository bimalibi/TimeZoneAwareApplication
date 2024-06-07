using MeetingScheduler.Data;
using MeetingScheduler.Entities;
using MeetingScheduler.Providers;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Endpoints;

public static class UserEndpoints
{
    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/user/me", (ICurrentUserProvider currentUserProvider, DbContext context) =>
        {
            var userId = currentUserProvider.GetCurrentUserId();
            var userTimeZone = context.Set<UserTimeZone>().FirstOrDefault(x => x.UserId == userId);
            if (userTimeZone == null) throw new Exception("Please set your timezone first.");
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