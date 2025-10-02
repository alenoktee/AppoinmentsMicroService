using AutoMapper;
using Appointments.Application.Commands;
using Appointments.Domain.Entities;

namespace Appointments.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAppointmentCommand, Appointment>();
    }
}
