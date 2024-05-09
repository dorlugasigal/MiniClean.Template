using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace Infrastructure.DataHub.Clients.Abstractions;

public interface IDataHubFhirClientWrapper
{
    Task<T> ReadAsync<T>(string resourceLocation) where T : Resource;
    Task<T> UpdateAsync<T>(T resource) where T : Resource;
    Task<T> CreateResource<T>(T resource) where T : Resource;
    Task<Bundle> TransactionAsync<T>(Bundle bundle) where T : Resource;
    Task<Bundle> SearchResourceByIdentifier<T>(string identifier) where T : Resource, new();
    Task<Bundle> SearchResourceByParams<T>(SearchParams searchParams) where T : Resource, new();
    Task<Bundle?> ContinueAsync(Bundle current);
}