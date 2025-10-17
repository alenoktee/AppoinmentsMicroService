using Appointments.Application.Exceptions;
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
        var appointment = await _appointmentsRepository.GetByIdAsync(request.AppointmentId);
        if (appointment is null)
        {
            throw new NotFoundException($"Appointment with ID {request.AppointmentId} not found.");
        }

        var result = await _resultsRepository.CreateAsync(_mapper.Map<Result>(request));

        await _appointmentsRepository.ChangeStatusAsync(request.AppointmentId, (short)AppointmentStatus.Completed);

        await _notificationService.SendResultUpdateNotificationAsync(result, appointment, cancellationToken);

        return result.Id;
    }
}
