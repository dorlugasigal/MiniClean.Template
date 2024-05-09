using Core;
using Core.Abstractions.Clients;
using Infrastructure.DataHub;
using Infrastructure.DataHub.Clients.Abstractions;
using Infrastructure.DataHub.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Tests.Infrastructure.DataHub;

public class DependencyInjectionTests
{
    [Fact]
    public void AddDataHubFhirInfrastructure_ShouldThrowException_WhenFhirServerConfigurationIsMissing()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        Should.Throw<Exception>(() => services.AddDataHubFhirInfrastructure(configuration));
    }

    [Fact]
    public void AddDataHubInfrastructure_ShouldAddServices()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Local.json")
            .Build();
        var services = new ServiceCollection();
        services.AddCore();
        services.AddDataHubFhirInfrastructure(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var dataHubFhirClient = serviceProvider.GetService<IDataHubFhirClient>();
        var dataHubFhirClientWrapper = serviceProvider.GetService<IDataHubFhirClientWrapper>();
        var dataHubFhirConfig = serviceProvider.GetService<DataHubFhirServerConfiguration>();

        dataHubFhirClient.ShouldNotBeNull();
        dataHubFhirClientWrapper.ShouldNotBeNull();
        dataHubFhirConfig.ShouldNotBeNull();
    }
}