using Appointments.Application.Configuration;
using Appointments.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace Appointments.Application.Appointments.Queries.GetFreeSlots;

public class GetFreeSlotsQueryHandler : IRequestHandler<GetFreeSlotsQuery, IEnumerable<TimeSpan>>
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly WorkScheduleSettings _workScheduleSettings;

    public GetFreeSlotsQueryHandler(IAppointmentsRepository appointmentsRepository, IOptions<WorkScheduleSettings> workScheduleSettings)
    {
        _appointmentsRepository = appointmentsRepository;
        _workScheduleSettings = workScheduleSettings.Value;
    }

    public async Task<IEnumerable<TimeSpan>> Handle(GetFreeSlotsQuery request, CancellationToken cancellationToken)
    {
        var occupiedSlots = await _appointmentsRepository.GetOccupiedTimeSlotsAsync(request.DoctorId, request.Date);

        var allPossibleSlots = _workScheduleSettings.GenerateAllPossibleSlots();

        var freeSlots = allPossibleSlots
            .Where(slot =>
                !occupiedSlots.Any(occupied =>
                    slot >= occupied.StartTime && slot < occupied.EndTime))
            .ToList();

        return freeSlots;
    }
}
