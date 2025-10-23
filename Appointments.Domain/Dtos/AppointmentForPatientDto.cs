namespace Appointments.Application.Dtos;

public record AppointmentForPatientDto(
    Guid Id,
    DateTime Date,
    TimeSpan Time,
    string DoctorFirstName,
    string DoctorLastName,
    string ServiceName,
    short Status
);
