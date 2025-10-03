using Appointments.Application.Dtos;

using MediatR;

namespace Appointments.Application.Appointments.Queries.GetAppointmentAsPatient;

public record GetAppointmentAsPatientQuery(Guid Id) : IRequest<AppointmentForPatientDto?>;
