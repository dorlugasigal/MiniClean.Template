using System.Diagnostics.CodeAnalysis;
using Infrastructure.Common.HealthCheck;

namespace Infrastructure.DataHub.HealthCheck;

[ExcludeFromCodeCoverage]
public class DataHubFhirHealthCheck(IHttpClientFactory clientFactory) : BaseHealthCheck(clientFactory)
{
    protected override string ClientName => "DataHubFhirClient";
    protected override string HealthCheckEndpoint => "/health/check";
}