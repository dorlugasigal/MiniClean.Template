namespace E2E.Tests;

[Trait("Category", "Smoke")]
public class HealthCheckTests(ITestOutputHelper outputHelper) : BaseApiTest(outputHelper)
{
    [Fact]
    public async Task WhenCallingHealthCheck_ThenServiceAndAllDependentServicesShouldBeHealthy()
    {
        var response = await RetryUntilSuccessful(
            action: () => ApiClient.Execute(Get("/_health")),
            maxRetryAttempts: 6,
            secondsBetweenFailures: 10
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = JToken.Parse(response.Content!);
        content.Value<string>("status").ShouldBe("Healthy");
        content.SelectTokens("entries.*.status").All(s => s.Value<string>() == "Healthy").ShouldBeTrue();
    }
}