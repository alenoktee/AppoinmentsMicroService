namespace Appointments.Application.DTOs;

public record AppointmentForDoctorDto(
    Guid Id,
    DateTime Date,
    TimeSpan Time,
    string PatientFirstName,
    string PatientLastName,
    string ServiceName
);
