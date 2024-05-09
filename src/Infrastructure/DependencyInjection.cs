using System.Net;
using Infrastructure.DataHub;
using Infrastructure.DataHub.HealthCheck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDataHubFhirInfrastructure(configuration)
            .AddInfrastructureHealthChecks();
    }

    private static IServiceCollection AddInfrastructureHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DataHubFhirHealthCheck>("DataHub FHIR Health Check", tags: ["FHIR", "DataHub", "Api"]);

        return services;
    }
}