using Appointments.Application.Appointments.Commands.CreateAppointment;
using Appointments.Application.Dtos;
using Appointments.Domain.Entities;

using AutoMapper;

namespace Appointments.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAppointmentDto, CreateAppointmentCommand>();
        CreateMap<CreateAppointmentCommand, Appointment>()
            .ForMember(dest => dest.OfficeId, opt => opt.MapFrom(src => src.OfficeId));

        CreateMap<Appointment, AppointmentForDoctorDto>();
        CreateMap<Appointment, AppointmentForPatientDto>();
        CreateMap<Appointment, AppointmentForReceptionistDto>();
    }
}
