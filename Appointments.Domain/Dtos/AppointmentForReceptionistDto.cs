namespace Appointments.Application.Dtos;

public record AppointmentForReceptionistDto(
    Guid Id,
    DateTime Date,
    TimeSpan Time,
    string PatientFirstName,
    string PatientLastName,
    string? PatientMiddleName,
    string DoctorFirstName,
    string DoctorLastName,
    string? DoctorMiddleName, 
    string ServiceName,
    string PatientPhoneNumber,
    short Status,
    Guid OfficeId
);
