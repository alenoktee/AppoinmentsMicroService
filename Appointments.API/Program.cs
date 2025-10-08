using Appointments.API.Consumers;
using Appointments.API.Infrastructure.Data;
using Appointments.Application.Appointments.Commands.CreateAppointment;
using Appointments.Application.Mappings;
using Appointments.Domain.Interfaces;
using Appointments.Infrastructure.Repositories;

using MassTransit;

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
        builder.Services.AddScoped<IAppointmentsRepository, AppointmentsRepository>();

        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(CreateAppointmentCommandHandler).Assembly);
        });

        builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

        builder.Services.AddScoped<IAppointmentsRepository, AppointmentsRepository>();
        builder.Services.AddScoped<IResultsRepository, ResultsRepository>();

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
