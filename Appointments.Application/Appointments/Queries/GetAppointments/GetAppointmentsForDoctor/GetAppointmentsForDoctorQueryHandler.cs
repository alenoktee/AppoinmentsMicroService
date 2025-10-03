using Appointments.Application.Dtos;
using Appointments.Domain.Interfaces;

using MediatR;

namespace Appointments.Application.Appointments.Queries.GetAppointments.GetAppointmentsForDoctor;

public class GetAppointmentsForDoctorQueryHandler : IRequestHandler<GetAppointmentsForDoctorQuery, IEnumerable<AppointmentForDoctorDto>>
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public GetAppointmentsForDoctorQueryHandler(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task<IEnumerable<AppointmentForDoctorDto>> Handle(GetAppointmentsForDoctorQuery request, CancellationToken cancellationToken)
    {
        return await _appointmentsRepository.GetForDoctorPaginatedAsync(request.DoctorId, request.PageSize, request.PageNumber, request.Date);
    }
}
