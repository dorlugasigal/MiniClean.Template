using System.Net;
using Infrastructure.Common.HealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Task = System.Threading.Tasks.Task;

namespace Unit.Tests.Infrastructure.Common.HealthCheck;

public static class HttpClientMocker
{
    public static HttpClient SetupHttpClient(IHttpClientFactory factory, HttpStatusCode statusCode, string serializedResponse = "")
    {
        var messageHandler = new HttpMessageHandlerStub((_, _) =>
            Task.FromResult(new HttpResponseMessage { StatusCode = statusCode, Content = new StringContent(serializedResponse) }));

        var httpClient = new HttpClient(messageHandler);
        httpClient.BaseAddress = new Uri("http://localhost");
        factory.CreateClient(Arg.Any<string>()).Returns(httpClient);
        return httpClient;
    }
}

public class MockHealthCheck(IHttpClientFactory clientFactory) : BaseHealthCheck(clientFactory)
{
    protected override string ClientName => "HttpClient";
    protected override string HealthCheckEndpoint => "/health";
}

public class HealthCheckTestBase
{
    private readonly IHttpClientFactory _clientFactory = Substitute.For<IHttpClientFactory>();
    private MockHealthCheck Sut { get; }

    private void SetupHttpClient(HttpStatusCode statusCode)
    {
        HttpClientMocker.SetupHttpClient(_clientFactory, statusCode);
    }

    public HealthCheckTestBase()
    {
        Sut = new MockHealthCheck(_clientFactory);
    }

    [Fact]
    protected async Task CheckDataHubHealthAsync_ReturnsHealthy_WhenResponseIsSuccessfulAndHealthy()
    {
        SetupHttpClient(HttpStatusCode.OK);

        var result = await Sut.CheckHealthAsync(new HealthCheckContext());

        result.Status.ShouldBe(HealthStatus.Healthy);
    }

    [Fact]
    protected async Task CheckHealthAsync_ReturnsUnhealthy_WhenResponseIsUnsuccessful()
    {
        SetupHttpClient(HttpStatusCode.ServiceUnavailable);

        var result = await Sut.CheckHealthAsync(new HealthCheckContext());

        result.Status.ShouldBe(HealthStatus.Unhealthy);
    }
}