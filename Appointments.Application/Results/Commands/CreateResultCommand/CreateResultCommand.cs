using MediatR;

namespace Appointments.Application.Results.Commands.CreateResultCommand;

public record CreateResultCommand(
    Guid AppointmentId,
    string Complaints,
    string Conclusion,
    string Recommendations
) : IRequest<Guid>, IResultCommand;
