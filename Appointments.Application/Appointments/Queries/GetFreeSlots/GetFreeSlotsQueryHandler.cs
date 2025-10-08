using Appointments.Domain.Interfaces;

using MediatR;

namespace Appointments.Application.Appointments.Queries.GetFreeSlots;

public class GetFreeSlotsQueryHandler : IRequestHandler<GetFreeSlotsQuery, IEnumerable<TimeSpan>>
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public GetFreeSlotsQueryHandler(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task<IEnumerable<TimeSpan>> Handle(GetFreeSlotsQuery request, CancellationToken cancellationToken)
    {
        var occupiedSlots = await _appointmentsRepository.GetOccupiedTimeSlotsAsync(request.DoctorId, request.Date);

        var workDayStart = new TimeSpan(9, 0, 0);
        var workDayEnd = new TimeSpan(18, 0, 0);
        var slotDuration = TimeSpan.FromMinutes(10);

        var allPossibleSlots = new List<TimeSpan>();
        var currentTime = workDayStart;
        while (currentTime < workDayEnd)
        {
            allPossibleSlots.Add(currentTime);
            currentTime = currentTime.Add(slotDuration);
        }

        var freeSlots = new List<TimeSpan>(allPossibleSlots);

        foreach (var occupied in occupiedSlots)
        {
            freeSlots.RemoveAll(slot =>
                slot >= occupied.StartTime && slot < occupied.EndTime);
        }

        return freeSlots;
    }
}
