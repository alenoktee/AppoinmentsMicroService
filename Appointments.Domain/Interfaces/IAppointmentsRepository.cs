using Appointments.Application.Dtos;
using Appointments.Domain.Dtos;
using Appointments.Domain.Entities;

namespace Appointments.Domain.Interfaces;

public interface IAppointmentsRepository
{
    Task<Guid> CreateAsync(Appointment appointment);
    Task<IEnumerable<AppointmentForDoctorDto>> GetForDoctorPaginatedAsync(Guid doctorId, int pageSize, int pageNumber, DateTime date);
    Task<IEnumerable<AppointmentForPatientDto>> GetForPatientPaginatedAsync(Guid patientId, int pageSize, int pageNumber);
    Task<IEnumerable<AppointmentForReceptionistDto>> GetForReceptionistPaginatedAsync(
            int pageSize, int pageNumber, DateTime? date, string? doctorFullName,
            string? serviceName, short? status, Guid? officeId);
    Task<int> ApproveAsync(Guid id);
    Task<int> ChangeStatusAsync(Guid id, short status);
    Task<int> RescheduleAsync(Guid id, DateTime newDate, TimeSpan newTime);
    Task<AppointmentForDoctorDto?> GetForDoctorByIdAsync(Guid id);
    Task<AppointmentForPatientDto?> GetForPatientByIdAsync(Guid id);
    Task<AppointmentForReceptionistDto?> GetForReceptionistByIdAsync(Guid id);
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<IEnumerable<OccupiedTimeSlotDto>> GetOccupiedTimeSlotsAsync(Guid doctorId, DateTime date);
    Task UpdateServiceNameInAppointments(Guid serviceId, string newName);
}
