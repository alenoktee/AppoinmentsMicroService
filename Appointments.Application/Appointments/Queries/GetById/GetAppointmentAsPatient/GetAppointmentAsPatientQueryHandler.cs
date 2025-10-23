using AutoMapper;
using MediatR;
using Appointments.Domain.Interfaces;
using Appointments.Application.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace Appointments.Application.Appointments.Queries.GetById.GetAppointmentAsPatient;

public class GetAppointmentAsPatientQueryHandler : IRequestHandler<GetAppointmentAsPatientQuery, AppointmentForPatientDto?>
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IMapper _mapper;

    public GetAppointmentAsPatientQueryHandler(IAppointmentsRepository appointmentsRepository, IMapper mapper)
    {
        _appointmentsRepository = appointmentsRepository;
        _mapper = mapper;
    }

    public async Task<AppointmentForPatientDto?> Handle(GetAppointmentAsPatientQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentsRepository.GetForPatientByIdAsync(request.Id);
        return appointment;
    }
}
