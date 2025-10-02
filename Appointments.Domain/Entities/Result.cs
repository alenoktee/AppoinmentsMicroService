using System;
using System.Collections.Generic;

namespace Appointments.Domain.Entities;

public partial class Result
{
    public Guid Id { get; init; }
    public string Complaints { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public string Recommendations { get; set; } = string.Empty;
    public Guid AppointmentId { get; set; }
    public virtual Appointment Appointment { get; set; } = new Appointment();
}
