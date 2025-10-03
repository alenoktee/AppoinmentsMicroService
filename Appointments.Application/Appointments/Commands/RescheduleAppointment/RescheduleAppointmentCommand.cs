using MediatR;

namespace Appointments.Application.Appointments.Commands.RescheduleAppointment;

public record RescheduleAppointmentCommand(
    Guid Id,
    DateTime NewDate,
    TimeSpan NewTime
    ) : IRequest;
