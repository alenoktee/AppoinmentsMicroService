using MediatR;

namespace Appointments.Application.Appointments.Commands.CreateAppointment;

public record CreateAppointmentCommand(
    Guid PatientId,
    Guid DoctorId,
    Guid ServiceId,
    DateTime Date,
    TimeSpan Time,
    string ServiceName,
    string DoctorFirstName,
    string DoctorLastName,
    string? DoctorMiddleName,
    string PatientFirstName,
    string PatientLastName,
    string? PatientMiddleName
) : IRequest<Guid>;
