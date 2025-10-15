using Appointments.Application.Dtos;
using MediatR;

namespace Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForPatient;

public record GetAppointmentsForPatientQuery(Guid PatientId, int PageSize, int PageNumber) : IRequest<IEnumerable<AppointmentForPatientDto>>;
