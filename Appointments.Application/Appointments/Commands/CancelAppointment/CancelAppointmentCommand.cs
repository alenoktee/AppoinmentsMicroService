using MediatR;

namespace Appointments.Application.Appointments.Commands.CancelAppointment;

public record CancelAppointmentCommand(Guid Id) : IRequest;
