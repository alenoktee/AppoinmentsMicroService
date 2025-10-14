using Appointments.Application.Exceptions;
using Appointments.Domain.Interfaces;

using MediatR;

using Npgsql;

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
        try
        {
            await _appointmentsRepository.RescheduleAsync(request.Id, request.NewDate, request.NewTime);
        }
        catch (PostgresException ex) when (ex.SqlState == "P0001")
        {
            throw new BadRequestException(ex.Message);
        }
        catch (PostgresException ex) when (ex.SqlState == "P0002")
        {
            throw new NotFoundException(ex.Message);
        }
    }
}
