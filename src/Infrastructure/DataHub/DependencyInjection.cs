using Azure.Identity;
using Core.Abstractions.Clients;
using Hl7.Fhir.Rest;
using Infrastructure.DataHub.Clients;
using Infrastructure.DataHub.Clients.Abstractions;
using Infrastructure.DataHub.Configuration;
using Infrastructure.DataHub.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DataHub;

public static class DependencyInjection
{
    public static IServiceCollection AddDataHubFhirInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dataHubFhirServerConfiguration = configuration.GetSection(DataHubFhirServerConfiguration.SectionKey)
                                                 .Get<DataHubFhirServerConfiguration>()
                                             ?? throw new Exception("FhirServerConfiguration is not configured.");

        services.AddSingleton(dataHubFhirServerConfiguration)
            .AddDataHubFhirClient(dataHubFhirServerConfiguration);

        return services;
    }

    private static IServiceCollection AddDataHubFhirClient(this IServiceCollection services,
        DataHubFhirServerConfiguration configuration)
    {
        var baseUrl = configuration.BaseUrl;
        var settings = new FhirClientSettings
        {
            PreferredFormat = ResourceFormat.Json,
            ReturnPreference = ReturnPreference.Representation,
            VerifyFhirVersion = false,
            PreferredParameterHandling = SearchParameterHandling.Lenient
        };


        services.AddHttpClient("DataHubFhirClient")
            .ConfigurePrimaryHttpMessageHandler(_ =>
            {
                var handler = new HttpClientHandler
                {
                    CheckCertificateRevocationList = true
                };
                return new HttpClientAuthenticationHandler(handler, configuration.Authentication, new DefaultAzureCredential());
            })
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(baseUrl));


        services.AddTransient<IDataHubFhirClientWrapper>(ctx =>
        {
            var httpClient = ctx.GetRequiredService<IHttpClientFactory>()
                .CreateClient("DataHubFhirClient");
            var fhirClient = new FhirClient(baseUrl, httpClient, settings);
            return new DataHubFhirClientWrapper(fhirClient);
        });

        services.AddTransient<IDataHubFhirClient, DataHubFhirClient>();
        return services;
    }
}