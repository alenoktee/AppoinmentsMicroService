namespace Appointments.Application.Services.Interfaces;

public interface IProfileServiceClient
{
    Task<Guid?> GetAccountIdByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
}
