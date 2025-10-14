using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Domain.Enums;
using Appointments.Domain.Interfaces;

using AutoMapper;

using MediatR;

namespace Appointments.Application.Results.Commands.CreateResultCommand;

public class CreateResultCommandHandler : IRequestHandler<CreateResultCommand, Guid>
{
    private readonly IResultsRepository _resultsRepository;
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public CreateResultCommandHandler(IResultsRepository resultsRepository, IAppointmentsRepository appointmentsRepository, IMapper mapper, INotificationService notificationService)
    {
        _resultsRepository = resultsRepository;
        _appointmentsRepository = appointmentsRepository;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<Guid> Handle(CreateResultCommand request, CancellationToken cancellationToken)
    {
        var result = _mapper.Map<Result>(request);
        var resultId = await _resultsRepository.CreateAsync(result);
        await _appointmentsRepository.ChangeStatusAsync(request.AppointmentId, (short)AppointmentStatus.Completed);

        var createdResult = await _resultsRepository.GetByIdAsync(resultId);
        if (createdResult != null)
        {
            await _notificationService.SendResultUpdateNotificationAsync(createdResult, cancellationToken);
        }

        return resultId;
    }
}
