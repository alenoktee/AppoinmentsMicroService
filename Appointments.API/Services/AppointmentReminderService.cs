
using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Interfaces;
using Appointments.Infrastructure.Repositories;

namespace Appointments.API.Services;

public class AppointmentReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AppointmentReminderService> _logger;
    private static readonly TimeSpan Period = TimeSpan.FromSeconds(30);

    public AppointmentReminderService(IServiceProvider serviceProvider, ILogger<AppointmentReminderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Period);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ProcessRemindersAsync(stoppingToken);
        }
    }

    private async Task ProcessRemindersAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var appointmentsRepository = scope.ServiceProvider.GetRequiredService<IAppointmentsRepository>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        try
        {
            var tomorrow = DateTime.UtcNow.AddDays(1);
            _logger.LogInformation("Processing appointment reminders for {Date}", tomorrow);
            var appointments = await appointmentsRepository.GetAppointmentsForDateAsync(tomorrow);
            _logger.LogInformation("Found {Count} appointments for tomorrow", appointments.Count());

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
