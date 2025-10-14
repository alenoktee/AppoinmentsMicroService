using Appointments.API.Consumers;
using Appointments.API.Infrastructure.Data;
using Appointments.API.Services;
using Appointments.Application.Appointments.Commands.CreateAppointment;
using Appointments.Application.Behaviors;
using Appointments.Application.Mappings;
using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Interfaces;
using Appointments.Infrastructure.Repositories;
using Appointments.Infrastructure.Services;
using Appointments.Application.Configuration;
using FluentValidation;

using MassTransit;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using System.Text;

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

        builder.Services.AddScoped<IAppointmentsRepository, AppointmentsRepository>();
        builder.Services.AddScoped<IResultsRepository, ResultsRepository>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddHostedService<AppointmentReminderService>();

        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(CreateAppointmentCommandHandler).Assembly);

            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        builder.Services.AddValidatorsFromAssembly(typeof(CreateAppointmentCommandHandler).Assembly);

        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(CreateAppointmentCommandHandler).Assembly);
        });

        builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<ServiceUpdatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h => {
                    h.Username("guest");
                    h.Password("guest");
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
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}
