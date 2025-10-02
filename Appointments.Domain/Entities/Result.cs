using System;
using System.Collections.Generic;

namespace Appointments.Infrastructure.Data.Models;

public partial class Result
{
    public Guid Id { get; init; }
    public string? Complaints { get; set; }
    public string? Conclusion { get; set; }
    public string? Recomendations { get; set; }
    public Guid AppointmentId { get; set; }
    public virtual Appointment Appointment { get; set; } = null!;
}
