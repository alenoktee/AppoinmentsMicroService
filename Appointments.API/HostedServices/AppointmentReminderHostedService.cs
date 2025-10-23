using Appointments.Application.Configuration;
using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Appointments.API.HostedServices;

public class AppointmentReminderHostedService : BackgroundService
{
    private readonly ILogger<AppointmentReminderHostedService> _logger;
    private readonly TimeSpan _period;
    private readonly IServiceScopeFactory _scopeFactory;

    public AppointmentReminderHostedService(IOptions<ReminderSettings> reminderSettings, ILogger<AppointmentReminderHostedService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _period = reminderSettings.Value.CheckInterval;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(_period);

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
        {
            await ProcessRemindersAsync(cancellationToken);
        }
    }

    private async Task ProcessRemindersAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

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
