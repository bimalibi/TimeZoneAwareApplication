namespace MeetingScheduler.Providers;

public interface ICurrentUserProvider
{
    long GetCurrentUserId();
    string UserTimeZone();
    DateTime GetDateTime(DateTime utcDate);
}