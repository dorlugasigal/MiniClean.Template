using Core.Abstractions.Clients;
using DotNetEnv;
using Integration.Tests.DataProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Tests;

internal sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public ApiWebApplicationFactory()
    {
        Env.Load(TestPaths.EnvFilePath);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Testing");

        builder.ConfigureTestServices(services =>
        {
            var dataHubFhirClient = services.BuildServiceProvider().GetRequiredService<IDataHubFhirClient>();
            SeedDataProvider.RegisterSeedData(dataHubFhirClient);
        });
    }
}