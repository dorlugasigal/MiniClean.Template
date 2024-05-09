using System.Net;
using Core.Abstractions.Clients;
using Core.Abstractions.Services;
using Core.Results;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public class PatientsService(ILogger<PatientsService> logger, IDataHubFhirClient fhirClient) : IPatientsService
{
    public async Task<Result<Patient>> GetById(string id)
    {
        logger.LogDebug($"Fetching patient with id: {id}");
        var getResult = await fhirClient.GetResource<Patient>(id);
        if (getResult.IsSuccess)
        {
            logger.LogDebug($"Patient with id: {id} fetched successfully");
            return getResult;
        }

        logger.LogError(getResult.Exception, $"Error fetching patient with id: {id}");
        return getResult.Exception is not FhirOperationException { Status: HttpStatusCode.NotFound }
            ? new ApplicationException($"Error fetching data from fhir server", getResult.Exception)
            : getResult;
    }
}