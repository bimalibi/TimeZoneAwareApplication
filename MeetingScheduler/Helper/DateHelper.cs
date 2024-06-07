using MeetingScheduler.Entities;
using MeetingScheduler.Providers;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Helper;

public class DateHelper : IDateHelper
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly DbContext _context;

    public DateHelper(ICurrentUserProvider currentUserProvider, DbContext context)
    {
        _currentUserProvider = currentUserProvider;
        _context = context;
    }

    public DateTime? ToUtc(DateTime? dateTime = null)
    {
        if (dateTime == null) return null;

        var currentUserId = _currentUserProvider.GetCurrentUserId();
        var timeZone = _context.Set<UserTimeZone>().FirstOrDefault(x => x.UserId == currentUserId);
        if (timeZone == null) throw new Exception("Please set your timezone first.");
        var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone.TimeZone);
        // Convert the DateTime to UTC based on the user's time zone
        return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Unspecified), userTimeZone);
    }


    public DateTime ToLocal(long userId, DateTime dateTime)
    {
        var timeZone = _context.Set<UserTimeZone>().FirstOrDefault(x => x.UserId == userId);
        if (timeZone == null) throw new Exception("Please set your timezone first.");
        var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone.TimeZone);
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, userTimeZone);
    }
}