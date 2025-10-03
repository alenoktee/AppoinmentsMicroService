namespace Appointments.Application.Dtos;

public record CreateAppointmentDto(
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
);
