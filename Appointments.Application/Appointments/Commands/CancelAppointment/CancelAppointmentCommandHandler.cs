using Appointments.Domain.Enums;
using Appointments.Domain.Interfaces;

using MediatR;

namespace Appointments.Application.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand>
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public CancelAppointmentCommandHandler(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var affectedRows = await _appointmentsRepository.ChangeStatusAsync(request.Id, (short)AppointmentStatus.Cancelled);

        if (affectedRows == 0)
        {
            throw new Exception($"Appointment with Id {request.Id} not found.");
        }
    }
}
