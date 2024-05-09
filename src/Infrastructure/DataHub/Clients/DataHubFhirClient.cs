using System.Net;
using Core.Abstractions.Clients;
using Core.Results;
using FluentValidation;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Infrastructure.DataHub.Clients.Abstractions;
using Microsoft.Extensions.Logging;
using Id = Hl7.Fhir.Model.Id;

namespace Infrastructure.DataHub.Clients;

public class DataHubFhirClient(ILogger<DataHubFhirClient> logger, IDataHubFhirClientWrapper dataHubFhirClient) : IDataHubFhirClient
{
    public async Task<Result<T>> GetResource<T>(string resourceId) where T : Resource
    {
        var resourceType = ModelInfo.GetFhirTypeNameForType(typeof(T));
        logger.LogInformation("Fetching resource {ResourceType}/{ResourceId} from FHIR service.", resourceType, resourceId);

        try
        {
            var response = await dataHubFhirClient.ReadAsync<T>($"{resourceType}/{resourceId}");
            return response;
        }
        catch (FhirOperationException ex) when (ex.Status == HttpStatusCode.NotFound)
        {
            logger.LogDebug("Resource {ResourceType}/{ResourceId} not found in FHIR service.", resourceType, resourceId);
            return ex;
        }
        catch (Exception ex)
        {
            logger.LogError("Error fetching resource {ResourceType}/{ResourceId} from FHIR service: {ErrorMessage}", resourceType, resourceId, ex.Message);
            return ex;
        }
    }
    
    public async Task<Result<T>> UpdateResource<T>(T? resource) where T : Resource
    {
        if (resource == null)
        {
            logger.LogError("Error updating resource: input resource argument is null");
            return new ArgumentNullException(nameof(resource));
        }

        logger.LogInformation("Updating resource {ResourceType}/{ResourceId} to FHIR service.", resource.TypeName, resource.Id);

        try
        {
            var response = await dataHubFhirClient.UpdateAsync(resource);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError("Error updating resource {ResourceType}/{ResourceId} to FHIR service: {ErrorMessage}", resource.TypeName, resource.Id, ex.Message);
            return ex;
        }
    }

    public async Task<Result<Bundle>> TransactionAsync<T>(Bundle bundle) where T : Resource
    {
        if (bundle == null)
        {
            logger.LogError("Error creating transaction bundle: input bundle argument is null");
            return new ArgumentNullException(nameof(bundle));
        }

        bundle!.IdElement = new Id(Guid.NewGuid().ToString());

        logger.LogInformation("Transaction bundle with total of {Total}", bundle.Entry.Count);

        return await dataHubFhirClient.TransactionAsync<T>(bundle);
    }

    public async Task<Result<T>> CreateResource<T>(T resource) where T : Resource
    {
        resource.IdElement = null;

        logger.LogInformation("Creating resource {ResourceType} to FHIR service.", resource.TypeName);

        try
        {
            return await dataHubFhirClient.CreateResource(resource);
        }
        catch (Exception ex)
        {
            logger.LogError("Error creating resource {ResourceType} to FHIR service: {ErrorMessage}", resource.TypeName, ex.Message);
            return ex;
        }
    }

    public async Task<Result<T>> SearchResourceByIdentifier<T>(string identifier) where T : Resource, new()
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            logger.LogError("Error searching resource: input identifier argument is null or empty");
            return new ArgumentNullException(nameof(identifier));
        }

        var resourceType = ModelInfo.GetFhirTypeNameForType(typeof(T));

        logger.LogInformation("Fetching resource {ResourceType} with Identifier {Identifier} from FHIR service.", resourceType, identifier);

        try
        {
            var response = await dataHubFhirClient.SearchResourceByIdentifier<T>(identifier);

            var isResourceFound = response?.Entry is { Count: > 0 };
            if (isResourceFound)
            {
                return response!.Entry!.First().Resource as T;
            }

            var errorMessage = $"Resource {resourceType} with Identifier {identifier} not found in FHIR service.";
            logger.LogDebug(errorMessage);
            return new FhirOperationException(errorMessage, HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            logger.LogError("Error fetching resource {ResourceType} with Identifier {Identifier} from FHIR service: {ErrorMessage}", resourceType, identifier, ex.Message);
            return ex;
        }
    }

    public async Task<Result<Bundle>> SearchResourceByParams<T>(SearchParams searchParams) where T : Resource, new()
    {
        var resourceType = ModelInfo.GetFhirTypeNameForType(typeof(T));
        logger.LogInformation("Searching for resource {ResourceType} from FHIR service using search parameters: {SearchParams}", resourceType, searchParams.ToUriParamList());

        try
        {
            var resources = await dataHubFhirClient.SearchResourceByParams<T>(searchParams);

            var isResourceFound = resources?.Entry is { Count: > 0 };
            if (isResourceFound)
            {
                return resources;
            }

            logger.LogDebug("Resource {ResourceType} not found in FHIR service.", resourceType);
            return new FhirOperationException(
                $"Resource {resourceType} not found in FHIR service.", HttpStatusCode.NotFound);
        }
        catch (FhirOperationException ex) when (ex.Status == HttpStatusCode.BadRequest)
        {
            logger.LogError("Bad request searching for resource {ResourceType} from FHIR service using search parameters: {SearchParams}", resourceType,
                searchParams.ToUriParamList());
            return ex;
        }
        catch (Exception ex)
        {
            logger.LogError("Error fetching resource {ResourceType} from FHIR service: {ErrorMessage}", resourceType, ex.Message);
            return ex;
        }
    }

    public async Task<Result<Bundle>> ContinueAsync(Bundle current)
    {
        logger.LogInformation("Continue search on bundle of type {Type}, with total of {Total}", current.Type, current.Total);
        return await dataHubFhirClient.ContinueAsync(current);
    }
}