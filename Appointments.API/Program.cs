using Appointments.API.Consumers;
using Appointments.API.HostedServices;
using Appointments.API.Infrastructure.Data;
using Appointments.API.Middleware;
using Appointments.Application.Appointments.Commands.CreateAppointment;
using Appointments.Application.Behaviors;
using Appointments.Application.Configuration;
using Appointments.Application.Mappings;
using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Interfaces;
using Appointments.Infrastructure.Repositories;
using Appointments.Infrastructure.Services;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace Appointments.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }
        builder.Services.AddSingleton<DapperContext>();

        builder.Services.Configure<WorkScheduleSettings>(builder.Configuration.GetSection("WorkSchedule"));
        builder.Services.Configure<ReminderSettings>(builder.Configuration.GetSection("ReminderSettings"));
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(RabbitMqSettings.SectionName));

        builder.Services.AddTransient<ExceptionHandlingMiddleware>();

        builder.Services.AddScoped<IAppointmentsRepository, AppointmentsRepository>();
        builder.Services.AddScoped<IResultsRepository, ResultsRepository>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddHostedService<AppointmentReminderHostedService>();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateAppointmentCommandHandler).Assembly);

            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        builder.Services.AddValidatorsFromAssembly(typeof(CreateAppointmentCommandHandler).Assembly);


        builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<ServiceUpdatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqSettings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                cfg.ReceiveEndpoint("appointments-service-updated", e =>
                {
                    e.ConfigureConsumer<ServiceUpdatedConsumer>(context);
                });
            });
        });

        builder.Services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["Services:Profiles"]);
        })
        .AddPolicyHandler(
            HttpPolicyExtensions
            .HandleTransientHttpError()
            .RetryAsync(3, (exception, retryCount) =>
            {
                Console.WriteLine($"Retry {retryCount} due to {exception.Exception.Message}");
            })
        );

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.MapControllers();

        app.Run();
    }
}
