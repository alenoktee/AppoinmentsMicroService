using Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForDoctor;
using Appointments.Application.Dtos;
using Appointments.Domain.Entities;
using Appointments.Domain.Interfaces;

using MediatR;

namespace Appointments.Application.Results.Queries.GetResultByIdQuery;

public class GetResultByIdQueryHandler : IRequestHandler<GetResultByIdQuery, Result?>
{
    private readonly IResultsRepository _resultsRepository;

    public GetResultByIdQueryHandler(IResultsRepository resultsRepository)
    {
        _resultsRepository = resultsRepository;
    }

    public async Task<Result?> Handle(GetResultByIdQuery request, CancellationToken cancellationToken)
    {
        return await _resultsRepository.GetByIdAsync(request.Id);
    }
}
