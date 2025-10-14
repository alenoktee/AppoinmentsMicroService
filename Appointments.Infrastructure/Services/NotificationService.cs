using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Domain.Interfaces;
using Shared.Messaging.Contracts;

using MassTransit;

namespace Appointments.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IProfileServiceClient _profileClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IAppointmentsRepository _appointmentsRepository;

    public NotificationService(IProfileServiceClient profileClient, IPublishEndpoint publishEndpoint, IAppointmentsRepository appointmentsRepository)
    {
        _profileClient = profileClient;
        _publishEndpoint = publishEndpoint;
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task<Guid?> GetAccountIdSafeAsync(Guid patientId, CancellationToken cancellationToken)
    {
        try
        {
            var accountId = await _profileClient.GetAccountIdByPatientIdAsync(patientId, cancellationToken);
            if (!accountId.HasValue)
            {
                return null;
            }
            return accountId;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task SendResultUpdateNotificationAsync(Result result, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentsRepository.GetByIdAsync(result.AppointmentId);
        if (appointment == null) return;

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

        await _publishEndpoint.Publish(message, cancellationToken);
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
        await _publishEndpoint.Publish(message, cancellationToken);
    }
}
