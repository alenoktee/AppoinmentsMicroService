using Appointments.Application.Dtos;

using MediatR;

namespace Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForReceptionist;

public record GetAppointmentsForReceptionistQuery(
    int PageSize,
    int PageNumber,
    DateTime? Date,
    string? DoctorFullName,
    string? ServiceName,
    int? Status,
    Guid? OfficeId) : IRequest<IEnumerable<AppointmentForReceptionistDto>>;
