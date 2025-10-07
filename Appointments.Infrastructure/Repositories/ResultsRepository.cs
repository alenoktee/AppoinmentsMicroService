using Appointments.API.Infrastructure.Data;
using Appointments.Domain.Entities;
using Appointments.Domain.Interfaces;

using Dapper;

namespace Appointments.Infrastructure.Repositories;

public class ResultsRepository : IResultsRepository
{
    private readonly DapperContext _context;

    public ResultsRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAsync(Result result)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT create_result(@Id, @AppointmentId, @Complaints, @Conclusion, @Recommendations)";

        return await connection.ExecuteScalarAsync<Guid>(sql, result);
    }


    public IEnumerable<Result> GetByAppointmentId(Guid appointmentId)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT * FROM get_results_by_appointment_id(@appointmentId)";
        return connection.Query<Result>(sql, new { appointmentId });
    }

    public async Task<Result?> GetByIdAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT * FROM get_result_by_id(@result_id)";

        return await connection.QuerySingleOrDefaultAsync<Result>(sql, new { result_id = id });
    }

    public async Task<int> UpdateAsync(Guid id, string complaints, string conclusion, string recommendations)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT update_result(@Id, @Complaints, @Conclusion, @Recommendations)";

        return await connection.ExecuteAsync(sql, new { Id = id, complaints, conclusion, recommendations });
    }
}
