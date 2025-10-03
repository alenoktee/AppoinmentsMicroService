using Appointments.Application.Dtos;

using MediatR;

namespace Appointments.Application.Appointments.Queries.GetAppointmentAsReceptionist;

public record GetAppointmentAsReceptionistQuery(Guid Id) : IRequest<AppointmentForReceptionistDto?>;
