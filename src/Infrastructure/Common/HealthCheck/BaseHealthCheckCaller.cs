using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.Common.HealthCheck;

public abstract class BaseHealthCheck(IHttpClientFactory clientFactory) : IHealthCheck
{
    protected abstract string ClientName { get; }
    protected abstract string HealthCheckEndpoint { get; }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var client = clientFactory.CreateClient(ClientName);
        var response = await client.GetAsync(HealthCheckEndpoint, cancellationToken);

        return response.IsSuccessStatusCode
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy($"Health Check failed. Status code: {response.StatusCode}");
    }
}