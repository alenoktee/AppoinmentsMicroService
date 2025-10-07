using Appointments.Domain.Entities;

using MediatR;

namespace Appointments.Application.Results.Queries.GetResultByIdQuery;

public record GetResultByIdQuery(
    Guid Id
) : IRequest<Result?>;
