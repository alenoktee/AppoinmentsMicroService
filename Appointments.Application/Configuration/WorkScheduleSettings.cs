namespace Appointments.Application.Configuration;

public class WorkScheduleSettings
{
    public TimeSpan WorkDayStart { get; set; }
    public TimeSpan WorkDayEnd { get; set; }
    public int SlotDurationInMinutes { get; set; }
}
