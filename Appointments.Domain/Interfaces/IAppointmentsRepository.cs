using Appointments.Domain.Entities;

namespace Appointments.Domain.Interfaces;

public interface IAppointmentsRepository
{
    Task<Appointment> CreateAsync(Appointment appointment);
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Appointment>> GetAsync();
    Task<IEnumerable<Appointment>> GetAsDoctorAsync(Guid doctorId, int pageSize, int pageNumber);
    Task<IEnumerable<Appointment>> GetAsPatientAsync(Guid patientId);
    Task<int> ChangeStatusAsync(Guid id, short status);
    Task DeleteAsync(Guid id);
    Task<Appointment?> GetAppointmentWithResultAsync(Guid id);
}
