namespace Appointments.Domain.Dtos;

public record CreateResultDto(
    string AppointmentId,
    string Complaints,
    string Conclusion,
    string Recommendations
);
