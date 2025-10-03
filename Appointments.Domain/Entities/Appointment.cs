using Appointments.Domain.Enums;

using System;
using System.Collections.Generic;

namespace Appointments.Domain.Entities;

public partial class Appointment
{
    public Guid Id { get; init; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public AppointmentStatus Status { get; set; }
    public Guid OfficeId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string DoctorFirstName { get; set; } = string.Empty;
    public string DoctorLastName { get; set; } = string.Empty;
    public string DoctorMiddleName { get; set; } = string.Empty;
    public string PatientFirstName { get; set; } = string.Empty;
    public string PatientLastName { get; set; } = string.Empty;
    public string PatientMiddleName { get; set; } = string.Empty;

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();

    public Appointment() { }
    public Appointment(Guid id)
    {
        Id = id;
    }
}
