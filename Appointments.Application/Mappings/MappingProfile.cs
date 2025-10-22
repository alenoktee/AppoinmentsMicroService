using Appointments.Application.Appointments.Commands.CreateAppointment;
using Appointments.Application.Dtos;
using Appointments.Application.Results.Commands.CreateResultCommand;
using Appointments.Domain.Dtos;
using Appointments.Domain.Entities;
using AutoMapper;
using Shared.Messages.Contracts;

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
        CreateMap<Result, ResultXmlDto>();

        CreateMap<Appointment, AppointmentResultUpdatedEvent>()
            .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.ResultId, opt => opt.Ignore())
            .ForMember(dest => dest.PatientAccountId, opt => opt.Ignore())
            .ForMember(dest => dest.ResultFile, opt => opt.Ignore())
            .ForMember(dest => dest.ContentType, opt => opt.Ignore());

        CreateMap<CreateResultCommand, Result>();

        CreateMap<Appointment, AppointmentProxy>()
            .ConstructUsing((source, context) =>
            {
                var loader = context.Items["ResultsLoader"] as Func<ICollection<Result>>;
                return new AppointmentProxy(loader);
            });
    }
}
