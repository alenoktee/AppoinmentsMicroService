using Appointments.Domain.Entities;

namespace Appointments.Application.Services.Interfaces;

public interface INotificationService
{
    Task SendResultUpdateNotificationAsync(Result result, Appointment appointment, CancellationToken cancellationToken = default);
    Task SendAppointmentReminderNotificationAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task<Guid?> GetAccountIdSafeAsync(Guid patientId, CancellationToken cancellationToken = default); 
}
