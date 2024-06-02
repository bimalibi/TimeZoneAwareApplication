namespace MeetingScheduler.Contracts;

public class MeetingFilterPayload
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDateTime { get; set; }
}

public class MeetingPayload
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public long InvitedUserId { get; set; }
}