using AutoMapper;
using MediatR;
using Appointments.Domain.Interfaces;
using Appointments.Application.Dtos;

namespace Appointments.Application.Appointments.Queries.GetById.GetAppointmentAsDoctor;

public class GetAppointmentAsDoctorQueryHandler : IRequestHandler<GetAppointmentAsDoctorQuery, AppointmentForDoctorDto?>
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IMapper _mapper;

    public GetAppointmentAsDoctorQueryHandler(IAppointmentsRepository appointmentsRepository, IMapper mapper)
    {
        _appointmentsRepository = appointmentsRepository;
        _mapper = mapper;
    }

    public async Task<AppointmentForDoctorDto?> Handle(GetAppointmentAsDoctorQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentsRepository.GetForDoctorByIdAsync(request.Id);
        return appointment;
    }
}
