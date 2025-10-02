namespace Appointments.Application.DTOs;

public record AppointmentForPatientDto(
    Guid Id,
    DateTime Date,
    TimeSpan Time,
    string DoctorFirstName,
    string DoctorLastName,
    string ServiceName
);
