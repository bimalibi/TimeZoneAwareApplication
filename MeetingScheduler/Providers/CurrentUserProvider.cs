using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using MeetingScheduler.Data;

namespace MeetingScheduler.Providers;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _dbContext;

    public CurrentUserProvider(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public long GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            throw new Exception("User not authenticated");
        }

        return long.Parse(userId);
    }

    public string UserTimeZone()
    {
        var userId = GetCurrentUserId();
        var userTimeZone = _dbContext.UserTimeZones.FirstOrDefault(x => x.UserId == userId);
        return userTimeZone?.TimeZone ?? "UTC";
    }


    public DateTime GetDateTime(DateTime utcDate)
    {
        var userTimeZone = UserTimeZone();
        var localDateTime = GetDateTimeBasedOn(userTimeZone, utcDate);
        return DateTime.Parse(localDateTime);
    }

    private string GetDateTimeBasedOn(string timeZone, DateTime utcDate)
    {
        var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDate, userTimeZone);
        return localDateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}