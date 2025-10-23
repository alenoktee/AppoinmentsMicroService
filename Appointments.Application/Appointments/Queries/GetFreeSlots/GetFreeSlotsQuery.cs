using MediatR;

namespace Appointments.Application.Appointments.Queries.GetFreeSlots;

public record GetFreeSlotsQuery(
    Guid DoctorId,
    DateTime Date
) : IRequest<IEnumerable<TimeSpan>>;
