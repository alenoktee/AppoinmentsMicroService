using Appointments.Domain.Entities;

namespace Appointments.Domain.Interfaces;

public interface IResultsRepository
{
    Task<Result> CreateAsync(Result model);
    Task<Result?> GetByIdAsync(Guid id);
    Task UpdateAsync(Result model);
    Task DeleteAsync(Guid id);
}
