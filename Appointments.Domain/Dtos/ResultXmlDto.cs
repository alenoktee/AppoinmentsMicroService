using Appointments.Domain.Entities;

namespace Appointments.Domain.Dtos;

public class ResultXmlDto
{
    public string Complaints { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public string Recommendations { get; set; } = string.Empty;
}
