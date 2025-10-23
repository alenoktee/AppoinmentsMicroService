using MediatR;

namespace Appointments.Application.Appointments.Commands.ApproveAppointment;

public record ApproveAppointmentCommand(Guid Id) : IRequest;
