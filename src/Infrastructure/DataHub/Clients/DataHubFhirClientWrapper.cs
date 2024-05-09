using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Infrastructure.DataHub.Clients.Abstractions;

namespace Infrastructure.DataHub.Clients;

[ExcludeFromCodeCoverage]
public class DataHubFhirClientWrapper(FhirClient fhirClient) : IDataHubFhirClientWrapper
{
    public Task<T> ReadAsync<T>(string resourceLocation) where T : Resource
    {
        return fhirClient.ReadAsync<T>(resourceLocation)!;
    }

    public Task<T> UpdateAsync<T>(T resource) where T : Resource
    {
        return fhirClient.UpdateAsync(resource)!;
    }

    public Task<Bundle> TransactionAsync<T>(Bundle bundle) where T : Resource
    {
        return fhirClient.TransactionAsync(bundle)!;
    }

    public Task<T> CreateResource<T>(T resource) where T : Resource
    {
        return fhirClient.CreateAsync(resource)!;
    }

    public Task<Bundle> SearchResourceByIdentifier<T>(string identifier) where T : Resource, new()
    {
        return fhirClient.SearchAsync<T>([$"identifier={identifier}"])!;
    }

    public Task<Bundle> SearchResourceByParams<T>(SearchParams searchParams) where T : Resource, new()
    {
        return fhirClient.SearchAsync<T>(searchParams)!;
    }

    public Task<Bundle?> ContinueAsync(Bundle current)
    {
        return fhirClient.ContinueAsync(current);
    }
}