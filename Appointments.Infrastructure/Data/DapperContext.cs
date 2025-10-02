namespace Appointments.API.Infrastructure.Data;

public class DapperContext
{
    public string ConnectionString { get; }

    public DapperContext(string connectionString)
    {
        ConnectionString = connectionString;
    }
}
