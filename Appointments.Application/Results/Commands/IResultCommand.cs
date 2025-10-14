namespace Appointments.Application.Results.Commands;

public interface IResultCommand
{
    string Complaints { get; }
    string Conclusion { get; }
    string Recommendations { get; }
}
