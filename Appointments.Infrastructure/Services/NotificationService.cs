using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Dtos;
using Appointments.Domain.Entities;
using AutoMapper;
using MassTransit;
using Shared.Messages.Contracts;
using System.Text;
using System.Xml.Serialization;
using static MassTransit.ValidationResultExtensions;

namespace Appointments.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IProfileServiceClient _profileClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<NotificationService> _logger;
    private const string XmlContentType = "application/xml";
    private readonly IMapper _mapper;

    public NotificationService(IProfileServiceClient profileClient, IPublishEndpoint publishEndpoint, ILogger<NotificationService> logger, IMapper mapper)
    {
        _profileClient = profileClient;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _mapper = mapper;
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

    public async Task SendResultUpdateNotificationAsync(Domain.Entities.Result result, Appointment appointment, CancellationToken cancellationToken = default)
    {
        var accountId = await GetAccountIdSafeAsync(appointment.PatientId, cancellationToken);
        if (!accountId.HasValue) return;

        var xmlDto = _mapper.Map<ResultXmlDto>(result);

        byte[] xmlData;
        var serializer = new XmlSerializer(typeof(ResultXmlDto));
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new StreamWriter(memoryStream, new UTF8Encoding(false)))
            {
                serializer.Serialize(writer, xmlDto);
            }
            xmlData = memoryStream.ToArray();
        }

        var message = new AppointmentResultUpdatedEvent
        {
            ResultId = result.Id,
            PatientAccountId = accountId.Value,
            ResultFile = xmlData,
            ContentType = XmlContentType
        };

        _mapper.Map(appointment, message);

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
