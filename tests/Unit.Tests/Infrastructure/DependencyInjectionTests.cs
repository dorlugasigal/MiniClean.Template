using Core.Abstractions.Clients;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Tests.Infrastructure;

public class DependencyInjectionTests
{

    [Fact]
    public void AddInfrastructure_ShouldAddServices()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Local.json")
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var dataHubFhirClient = serviceProvider.GetService<IDataHubFhirClient>();

        dataHubFhirClient.ShouldNotBeNull();
    }
}