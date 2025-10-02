using AutoMapper;
using Appointments.Domain.Entities;
using Appointments.Application.Appointments.Commands.CreateAppointment;

namespace Appointments.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAppointmentCommand, Appointment>();
    }
}
