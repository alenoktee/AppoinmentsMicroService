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
            // P0001: User error (raise_exception).
            // Thrown when the doctor is unavailable for the selected time (record conflict).
            throw new BadRequestException(ex.Message);
        }
        catch (PostgresException ex) when (ex.SqlState == "P0002")
        {
            // P0002: User error (no_data_found).
            // For cases where the corresponding data is not found
            // (e.g., the patient or service was not found during the appointment).
            throw new NotFoundException(ex.Message);
        }
    }
}
