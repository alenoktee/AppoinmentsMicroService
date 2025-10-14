using MediatR;

namespace Appointments.Application.Results.Commands.UpdateResultCommand;

public record UpdateResultCommand(
    Guid Id,
    string Complaints,
    string Conclusion,
    string Recommendations
) : IRequest<Unit>, IResultCommand;
