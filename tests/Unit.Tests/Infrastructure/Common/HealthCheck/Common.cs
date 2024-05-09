namespace Unit.Tests.Infrastructure.Common.HealthCheck;

public class HttpMessageHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return sendAsync(request, cancellationToken);
    }
}