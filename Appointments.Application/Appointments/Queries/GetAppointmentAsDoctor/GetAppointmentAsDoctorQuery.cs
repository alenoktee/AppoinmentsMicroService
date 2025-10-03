using Appointments.Application.Dtos;

using MediatR;

namespace Appointments.Application.Appointments.Queries.GetAppointmentAsDoctor;

public record GetAppointmentAsDoctorQuery(Guid Id) : IRequest<AppointmentForDoctorDto?>;
