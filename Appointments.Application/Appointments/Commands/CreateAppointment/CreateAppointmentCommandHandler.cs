using Appointments.Domain.Entities;
using Appointments.Domain.Enums;
using Appointments.Domain.Interfaces;

using AutoMapper;

using MediatR;

namespace Appointments.Application.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Guid>
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IMapper _mapper;

    public CreateAppointmentCommandHandler(IAppointmentsRepository appointmentsRepository, IMapper mapper)
    {
        _appointmentsRepository = appointmentsRepository;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken)
    {
        var appointment = _mapper.Map<Appointment>(command);

        appointment.Status = AppointmentStatus.Scheduled;

        await _appointmentsRepository.CreateAsync(appointment);

        return appointment.Id;
    }
}
