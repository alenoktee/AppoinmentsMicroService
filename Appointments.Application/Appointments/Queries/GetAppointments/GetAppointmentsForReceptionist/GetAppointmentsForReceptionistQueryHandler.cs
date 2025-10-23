using MediatR;
using Appointments.Domain.Interfaces;
using Appointments.Application.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForReceptionist;

public class GetAppointmentsForReceptionistQueryHandler : IRequestHandler<GetAppointmentsForReceptionistQuery, IEnumerable<AppointmentForReceptionistDto>>
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public GetAppointmentsForReceptionistQueryHandler(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task<IEnumerable<AppointmentForReceptionistDto>> Handle(GetAppointmentsForReceptionistQuery request, CancellationToken cancellationToken)
    {
        return await _appointmentsRepository.GetForReceptionistPaginatedAsync(
            request.PageSize,
            request.PageNumber,
            request.Date,
            request.DoctorFullName,
            request.ServiceName,
            request.Status,
            request.OfficeId);
    }
}
