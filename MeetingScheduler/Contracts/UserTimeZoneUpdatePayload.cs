namespace MeetingScheduler.Contracts;

public class UserTimeZoneUpdatePayload
{
    public long UserId { get; set; }
    public string TimeZone { get; set; }
}