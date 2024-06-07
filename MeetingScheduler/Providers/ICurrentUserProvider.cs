namespace MeetingScheduler.Providers;

public interface ICurrentUserProvider
{
    long GetCurrentUserId();
}