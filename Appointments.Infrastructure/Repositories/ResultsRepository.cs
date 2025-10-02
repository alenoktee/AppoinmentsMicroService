using Appointments.Domain.Entities;
using Appointments.Domain.Interfaces;

namespace Appointments.Infrastructure.Repositories;

public class ResultsRepository : IResultsRepository
{
    public Task<Result> CreateAsync(Result model)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Result model)
    {
        throw new NotImplementedException();
    }
}
