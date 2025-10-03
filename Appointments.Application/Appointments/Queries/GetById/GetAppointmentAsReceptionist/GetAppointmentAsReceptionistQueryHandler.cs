using AutoMapper;
using MediatR;
using Appointments.Domain.Interfaces;
using Appointments.Application.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace Appointments.Application.Appointments.Queries.GetById.GetAppointmentAsReceptionist;

public class GetAppointmentAsReceptionistQueryHandler : IRequestHandler<GetAppointmentAsReceptionistQuery, AppointmentForReceptionistDto?>
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IMapper _mapper;

    public GetAppointmentAsReceptionistQueryHandler(IAppointmentsRepository appointmentsRepository, IMapper mapper)
    {
        _appointmentsRepository = appointmentsRepository;
        _mapper = mapper;
    }

    public async Task<AppointmentForReceptionistDto?> Handle(GetAppointmentAsReceptionistQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentsRepository.GetForReceptionistByIdAsync(request.Id);
        return appointment;
    }
}
