using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Domain.Interfaces;
using MassTransit;
using Shared.Messages.Contracts;

namespace Appointments.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IProfileServiceClient _profileClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IProfileServiceClient profileClient, IPublishEndpoint publishEndpoint, IAppointmentsRepository appointmentsRepository, ILogger<NotificationService> logger)
    {
        _profileClient = profileClient;
        _publishEndpoint = publishEndpoint;
        _appointmentsRepository = appointmentsRepository;
        _logger = logger;
    }

    public async Task<Guid?> GetAccountIdSafeAsync(Guid patientId, CancellationToken cancellationToken)
    {
        try
        {
            var accountId = await _profileClient.GetAccountIdByPatientIdAsync(patientId, cancellationToken);
            if (!accountId.HasValue)
            {
                _logger.LogWarning("Account ID not found for Patient ID {PatientId}.", patientId);
                return null;
            }
            return accountId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Account ID for Patient ID {PatientId}.", patientId);
            return null;
        }
    }

    public async Task SendResultUpdateNotificationAsync(Result result, Appointment appointment, CancellationToken cancellationToken = default)
    {
        var accountId = await GetAccountIdSafeAsync(appointment.PatientId, cancellationToken);
        if (!accountId.HasValue) return;

        var message = new AppointmentResultUpdatedEvent
        {
            ResultId = result.Id,
            PatientAccountId = accountId.Value,
            PatientFirstName = appointment.PatientFirstName,
            DoctorFirstName = appointment.DoctorFirstName,
            DoctorLastName = appointment.DoctorLastName,
            ServiceName = appointment.ServiceName,
            AppointmentDate = appointment.Date
        };

        try
        {
            await _publishEndpoint.Publish(message, cancellationToken);

            _logger.LogInformation("Successfully published {EventName} for Patient Account ID {PatientAccountId}", nameof(AppointmentResultUpdatedEvent), accountId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish {EventName} for Patient Account ID {PatientAccountId}", nameof(AppointmentResultUpdatedEvent), accountId.Value);
        }
    }

    public async Task SendAppointmentReminderNotificationAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        var accountId = await GetAccountIdSafeAsync(appointment.PatientId, cancellationToken);
        if (!accountId.HasValue) return;

        var message = new AppointmentReminderEvent
        {
            PatientAccountId = accountId.Value,
            PatientFirstName = appointment.PatientFirstName,
            AppointmentDate = appointment.Date,
            AppointmentTime = appointment.Time,
            DoctorFirstName = appointment.DoctorFirstName,
            DoctorLastName = appointment.DoctorLastName,
            ServiceName = appointment.ServiceName
        };
        try
        {
            await _publishEndpoint.Publish(message, cancellationToken);

            _logger.LogInformation("Successfully published {EventName} for Patient Account ID {PatientAccountId}", nameof(AppointmentReminderEvent), accountId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish {EventName} for Patient Account ID {PatientAccountId}", nameof(AppointmentReminderEvent), accountId.Value);
        }
    }
}
