using Core.Results;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace Core.Abstractions.Clients;

public interface IDataHubFhirClient
{
    Task<Result<T>> GetResource<T>(string resourceId) where T : Resource;
    Task<Result<T>> UpdateResource<T>(T? resource) where T : Resource;
    Task<Result<Bundle>> TransactionAsync<T>(Bundle bundle) where T : Resource;
    Task<Result<T>> CreateResource<T>(T resource) where T : Resource;
    Task<Result<T>> SearchResourceByIdentifier<T>(string identifier) where T : Resource, new();
    Task<Result<Bundle>> SearchResourceByParams<T>(SearchParams searchParams) where T : Resource, new();
    Task<Result<Bundle>> ContinueAsync(Bundle current);

}