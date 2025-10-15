namespace Appointments.Application.Configuration;

public class WorkScheduleSettings
{
    public TimeSpan WorkDayStart { get; set; }
    public TimeSpan WorkDayEnd { get; set; }
    public int SlotDurationInMinutes { get; set; }

    public TimeSpan SlotDuration => TimeSpan.FromMinutes(SlotDurationInMinutes);

    public IEnumerable<TimeSpan> GenerateAllPossibleSlots()
    {
        var allSlots = new List<TimeSpan>();
        var currentTime = WorkDayStart;

        while (currentTime < WorkDayEnd)
        {
            allSlots.Add(currentTime);
            currentTime = currentTime.Add(SlotDuration);
        }

        return allSlots;
    }
}
