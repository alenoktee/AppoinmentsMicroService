using Appointments.Domain.Entities;
using Appointments.Application.Dtos;

namespace Appointments.Domain.Interfaces;

public interface IAppointmentsRepository
{
    Task<Appointment> CreateAsync(Appointment appointment);
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Appointment>> GetAsync();
    Task<IEnumerable<AppointmentForDoctorDto>> GetForDoctorPaginatedAsync(Guid doctorId, int pageSize, int pageNumber, DateTime date);
    Task<IEnumerable<AppointmentForPatientDto>> GetForPatientPaginatedAsync(Guid patientId, int pageSize, int pageNumber);
    Task<IEnumerable<AppointmentForReceptionistDto>> GetForReceptionistPaginatedAsync(
            int pageSize, int pageNumber, DateTime? date, string? doctorFullName,
            string? serviceName, short? status, Guid? officeId);
    Task<int> ApproveAsync(Guid id);
    Task<int> ChangeStatusAsync(Guid id, short status);
    Task DeleteAsync(Guid id);
    Task<Appointment?> GetAppointmentWithResultAsync(Guid id);
    Task<AppointmentForDoctorDto?> GetForDoctorByIdAsync(Guid id);
    Task<AppointmentForPatientDto?> GetForPatientByIdAsync(Guid id);
    Task<AppointmentForReceptionistDto?> GetForReceptionistByIdAsync(Guid id);
}
