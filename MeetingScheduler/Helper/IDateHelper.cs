namespace MeetingScheduler.Helper;

public interface IDateHelper
{
    /// <summary>
    /// It returns converts any date to utc based on user timezone
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    DateTime? ToUtc(DateTime? dateTime = null);
    
    /// <summary>
    /// It returns converts any date to utc based on user timezone
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    DateTime ToLocal(long userId, DateTime dateTime);
}