using Appointments.Domain.Interfaces;

using MediatR;

using Microsoft.AspNetCore.Authentication;

namespace Appointments.Application.Results.Commands.UpdateResultCommand;

public class UpdateResultCommandHandler : IRequestHandler<UpdateResultCommand>
{
    private readonly IResultsRepository _resultsRepository;

    public UpdateResultCommandHandler(IResultsRepository resultsRepository)
    {
        _resultsRepository = resultsRepository;
    }

    public async Task Handle(UpdateResultCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Complaints) ||
            string.IsNullOrWhiteSpace(request.Conclusion) ||
            string.IsNullOrWhiteSpace(request.Recommendations))
        {
            throw new ArgumentException("Все поля должны быть заполнены.");
        }

        await _resultsRepository.UpdateAsync(
            request.Id,
            request.Complaints,
            request.Conclusion,
            request.Recommendations);
    }
}
