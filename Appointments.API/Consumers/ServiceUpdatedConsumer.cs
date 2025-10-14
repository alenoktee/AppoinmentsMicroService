using MassTransit;
using Appointments.Domain.Interfaces;
using Shared.Messaging.Contracts;

namespace Appointments.API.Consumers;

public class ServiceUpdatedConsumer : IConsumer<ServiceUpdated>
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public ServiceUpdatedConsumer(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task Consume(ConsumeContext<ServiceUpdated> context)
    {
        var message = context.Message;
        await _appointmentsRepository.UpdateServiceNameInAppointments(message.Id, message.ServiceName);
    }
}
