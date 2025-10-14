using Appointments.Domain.Entities;

namespace Appointments.Domain.Dtos;

public class ResultXmlDto
{
    public Guid Id { get; init; }
    public string Complaints { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public string Recommendations { get; set; } = string.Empty;
    public Guid AppointmentId { get; set; }
}
