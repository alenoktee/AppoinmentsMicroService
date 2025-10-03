using Appointments.Domain.Interfaces;

using MediatR;

namespace Appointments.Application.Appointments.Commands.RescheduleAppointment;

public class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand>
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public RescheduleAppointmentCommandHandler(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task Handle(RescheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        await _appointmentsRepository.RescheduleAsync(request.Id, request.NewDate, request.NewTime);
    }
}
