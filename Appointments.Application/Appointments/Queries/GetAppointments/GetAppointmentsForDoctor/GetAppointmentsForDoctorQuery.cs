using Appointments.Application.Dtos;
using MediatR;

namespace Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForDoctor;

public record GetAppointmentsForDoctorQuery(Guid DoctorId, int PageSize, int PageNumber, DateTime Date) : IRequest<IEnumerable<AppointmentForDoctorDto>>;
