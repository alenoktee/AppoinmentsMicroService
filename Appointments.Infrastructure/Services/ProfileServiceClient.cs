using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Dtos;

namespace Appointments.Infrastructure.Services;

public class ProfileServiceClient : IProfileServiceClient
{
    private readonly HttpClient _httpClient;

    public ProfileServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Guid?> GetAccountIdByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<PatientAccountIdDto>($"/api/patients/{patientId}/account-id", cancellationToken);
        return response?.AccountId;
    }
}
