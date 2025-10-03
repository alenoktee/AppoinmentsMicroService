using Appointments.Application.Appointments.Commands.CreateAppointment;
using Appointments.Application.Dtos;
using Appointments.Domain.Entities;

using AutoMapper;

namespace Appointments.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Appointment, AppointmentForDoctorDto>();
        CreateMap<Appointment, AppointmentForPatientDto>();
        CreateMap<Appointment, AppointmentForReceptionistDto>();
    }
}
