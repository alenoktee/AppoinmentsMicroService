using Appointments.Domain.Entities;

namespace Appointments.Domain.Interfaces;

public interface IResultsRepository
{
    Task<Guid> CreateAsync(Result result);
    Task<Result?> GetByIdAsync(Guid id);
    Task<int> UpdateAsync(Guid id, string complaints, string conclusion, string recommendations);
    IEnumerable<Result> GetByAppointmentId(Guid appointmentId);
}
