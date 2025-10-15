using Appointments.Application.Configuration;
using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Interfaces;

using Microsoft.Extensions.Options;

namespace Appointments.API.HostedServices;

public class AppointmentReminderHostedService : BackgroundService
{
    private readonly ILogger<AppointmentReminderHostedService> _logger;
    private readonly TimeSpan _period;

    public AppointmentReminderHostedService(IOptions<ReminderSettings> reminderSettings, ILogger<AppointmentReminderHostedService> logger)
    {
        _logger = logger;
        _period = reminderSettings.Value.CheckInterval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_period);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ProcessRemindersAsync(stoppingToken);
        }
    }

    private async Task ProcessRemindersAsync(CancellationToken cancellationToken)
    {
        var appointmentsRepository = scope.ServiceProvider.GetRequiredService<IAppointmentsRepository>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        try
        {
            var nextDay = DateTime.UtcNow.AddDays(1);
            _logger.LogDebug("Processing appointment reminders for {Date}", nextDay);
            var appointments = await appointmentsRepository.GetAppointmentsForDateAsync(nextDay);
            _logger.LogDebug("Found {Count} appointments for tomorrow", appointments.Count());

            foreach (var appointment in appointments)
            {
                await notificationService.SendAppointmentReminderNotificationAsync(appointment, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing appointment reminders.");
        }
    }
}
