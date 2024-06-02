namespace MeetingScheduler.Providers;

public interface ICurrentUserProvider
{
    long GetCurrentUserId();
    string UserTimeZone();
    DateTime ConvertDateTimeToUserTimeZone(DateTime utcDate);
    
    DateTime ConvertDateTimeToUserTimeZone(long userId, DateTime utcDate);
}