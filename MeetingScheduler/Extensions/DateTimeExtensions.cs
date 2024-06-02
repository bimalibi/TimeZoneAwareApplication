namespace MeetingScheduler.Extensions;

public static class DateTimeExtensions
{
    public static DateTime TryParseUtc(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc) return dateTime;
        return dateTime.ToUniversalTime();
    }
}