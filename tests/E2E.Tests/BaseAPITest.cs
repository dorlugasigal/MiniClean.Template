using HttpTracer;
using HttpTracer.Logger;
using Polly;
using RestSharp.Authenticators;

namespace E2E.Tests;

public abstract class BaseApiTest(ITestOutputHelper outputHelper) : IDisposable
{
    protected RestClient FhirClient { get; } = GetRestClient(GetFhirBaseUrl(), outputHelper);
    protected RestClient ApiClient { get; } = GetRestClient(GetApiBaseUrl(), outputHelper);

    private static string GetApiBaseUrl()
    {
        return Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://localhost:8000";
    }

    private static string GetFhirBaseUrl()
    {
        return Environment.GetEnvironmentVariable("FHIR_BASE_URL") ?? "http://localhost:8080";
    }

    private static RestClient GetRestClient(string uri, ITestOutputHelper outputHelper)
    {
        var restClientOptions = new RestClientOptions(new UriBuilder(uri).Uri)
        {
            ConfigureMessageHandler = handler =>
                new HttpTracerHandler(handler, new TestOutputLogger(outputHelper), HttpMessageParts.All)
        };
        var authToken = Environment.GetEnvironmentVariable("AUTH_TOKEN");
        if (authToken != null)
        {
            restClientOptions.Authenticator = new JwtAuthenticator(authToken);
        }
        return new RestClient(restClientOptions);
    }

    protected async Task<RestResponse> RetryUntilSuccessful(Func<RestResponse> action, int maxRetryAttempts = 3, int secondsBetweenFailures = 2)
    {
        var timeSpan = TimeSpan.FromSeconds(secondsBetweenFailures);
        var policy = Policy
            .HandleResult<RestResponse>(x => !x.IsSuccessful)
            .WaitAndRetryAsync(maxRetryAttempts, _ => timeSpan, (result, _, attempt, _) =>
            {
                outputHelper.WriteLine(
                    $"""
                     The request failed.
                     HttpStatusCode={result.Result.StatusCode}.
                     Waiting {timeSpan} seconds before retry.
                     Number attempt {attempt}.
                     Uri={result.Result.ResponseUri};
                     RequestResponse={result.Result.Content}
                     """);
            });

        return await policy.ExecuteAsync(() => Task.FromResult(action()));
    }

    sealed class TestOutputLogger(ITestOutputHelper outputHelper) : ILogger
    {
        public void Log(string message)
        {
            outputHelper.WriteLine(message);
        }
    }

    public void Dispose()
    {
        ApiClient.Dispose();
        FhirClient.Dispose();
        GC.SuppressFinalize(this);
    }
}