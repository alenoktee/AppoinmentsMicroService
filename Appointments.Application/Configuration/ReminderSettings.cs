namespace Appointments.Application.Configuration;

public class ReminderSettings
{
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromMinutes(1);
}
