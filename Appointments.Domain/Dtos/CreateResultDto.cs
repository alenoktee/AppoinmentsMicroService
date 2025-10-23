namespace Appointments.Domain.Dtos;

public record CreateResultDto(
    string Complaints,
    string Conclusion,
    string Recommendations
);
