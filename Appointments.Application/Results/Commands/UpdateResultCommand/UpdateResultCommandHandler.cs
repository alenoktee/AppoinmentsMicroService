using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Interfaces;

using MediatR;

using Microsoft.AspNetCore.Authentication;

namespace Appointments.Application.Results.Commands.UpdateResultCommand;

public class UpdateResultCommandHandler : IRequestHandler<UpdateResultCommand, Unit>
{
    private readonly IResultsRepository _resultsRepository;
    private readonly INotificationService _notificationService;

    public UpdateResultCommandHandler(IResultsRepository resultsRepository, INotificationService notificationService)
    {
        _resultsRepository = resultsRepository;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(UpdateResultCommand request, CancellationToken cancellationToken)
    {
        await _resultsRepository.UpdateAsync(
            request.Id,
            request.Complaints,
            request.Conclusion,
            request.Recommendations);

        var result = await _resultsRepository.GetByIdAsync(request.Id);
        if (result != null)
        {
            await _notificationService.SendResultUpdateNotificationAsync(result, cancellationToken);
        }

        return Unit.Value;
    }
}
