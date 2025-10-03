using Appointments.Application.Dtos;

using MediatR;

namespace Appointments.Application.Appointments.Queries.GetById.GetAppointmentAsDoctor;

public record GetAppointmentAsDoctorQuery(Guid Id) : IRequest<AppointmentForDoctorDto?>;
