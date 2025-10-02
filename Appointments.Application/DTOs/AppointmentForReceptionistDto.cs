namespace Appointments.Application.DTOs;

public record AppointmentForReceptionistDto(
    Guid Id,
    DateTime Date,
    TimeSpan Time,
    string PatientFirstName,
    string PatientLastName,
    string DoctorFirstName,
    string DoctorLastName,
    string ServiceName
);
