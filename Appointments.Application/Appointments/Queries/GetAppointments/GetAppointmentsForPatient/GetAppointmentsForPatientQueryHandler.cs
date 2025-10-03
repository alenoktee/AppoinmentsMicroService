using Appointments.Application.Dtos;
using Appointments.Domain.Interfaces;

using MediatR;

namespace Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForPatient;

public class GetAppointmentsForPatientQueryHandler : IRequestHandler<GetAppointmentsForPatientQuery, IEnumerable<AppointmentForPatientDto>>
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public GetAppointmentsForPatientQueryHandler(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task<IEnumerable<AppointmentForPatientDto>> Handle(GetAppointmentsForPatientQuery request, CancellationToken cancellationToken)
    {
        return await _appointmentsRepository.GetForPatientPaginatedAsync(request.PatientId, request.PageSize, request.PageNumber);
    }
}
