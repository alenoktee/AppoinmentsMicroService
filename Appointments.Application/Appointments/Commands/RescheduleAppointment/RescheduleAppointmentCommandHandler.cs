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
            // P0001: Пользовательская ошибка (raise_exception).
            // Выбрасывается, когда врач недоступен на выбранное время (конфликт записей).
            throw new BadRequestException(ex.Message);
        }
        catch (PostgresException ex) when (ex.SqlState == "P0002")
        {
            // P0002: Пользовательская ошибка (no_data_found).
            // Для случаев, когда связанные данные не найдены
            // (например, не найден пациент или услуга при создании записи).
            throw new NotFoundException(ex.Message);
        }
    }
}
