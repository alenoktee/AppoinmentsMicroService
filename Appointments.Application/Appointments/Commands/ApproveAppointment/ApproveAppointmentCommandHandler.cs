using Appointments.Application.Exceptions;
using Appointments.Domain.Entities;
using Appointments.Domain.Enums;
using Appointments.Domain.Interfaces;

using MediatR;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appointments.Application.Appointments.Commands.ApproveAppointment;

public class ApproveAppointmentCommandHandler : IRequestHandler<ApproveAppointmentCommand>
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public ApproveAppointmentCommandHandler(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task Handle(ApproveAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentsRepository.ApproveAsync(request.Id);

        if (appointment == 0)
        {
            throw new NotFoundException(nameof(Appointment), request.Id);
        }
    }
}
