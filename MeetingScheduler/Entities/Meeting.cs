namespace MeetingScheduler.Entities;

public class Meeting
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public long CreatorId { get; set; }
    public long InvitedUserId { get; set; }
}

public class UserTimeZone
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string TimeZone { get; set; }
}