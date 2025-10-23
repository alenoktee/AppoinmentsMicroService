using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Interfaces;
using MediatR;

namespace Appointments.Application.Results.Commands.UpdateResultCommand;

public class UpdateResultCommandHandler : IRequestHandler<UpdateResultCommand, Unit>
{
    private readonly IResultsRepository _resultsRepository;
    private readonly INotificationService _notificationService;
    private readonly IAppointmentsRepository _appointmentsRepository;

    public UpdateResultCommandHandler(IResultsRepository resultsRepository,  INotificationService notificationService, IAppointmentsRepository appointmentsRepository)
    {
        _resultsRepository = resultsRepository;
        _notificationService = notificationService;
        _appointmentsRepository = appointmentsRepository;
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
            var appointment = await _appointmentsRepository.GetByIdAsync(result.AppointmentId);
            if (appointment != null)
            {
                await _notificationService.SendResultUpdateNotificationAsync(result, appointment, cancellationToken);
            }
        }

        return Unit.Value;
    }
}
