using System.Net;
using System.Net.Http.Headers;
using Azure.Core;
using Azure.Identity;
using Infrastructure.DataHub.Configuration;
using Infrastructure.DataHub.Handlers;
using NSubstitute.ExceptionExtensions;

namespace Unit.Tests.Infrastructure.DataHub.Handlers;

public class HttpClientAuthenticationHandlerTests
{

    [Fact]
    public async Task WhenSendAsyncCalled_TokenCredentialResponseIsSetAsBearerAuthorizationHeader()
    {
        var accessToken = "token";
        var scope = "scope";
        var tokenCredentialMock = Substitute.For<DefaultAzureCredential>();
        tokenCredentialMock.GetTokenAsync(Arg.Is<TokenRequestContext>(t => t.Scopes.Length == 1 && t.Scopes.First() == scope))
            .Returns(new AccessToken(accessToken, DateTimeOffset.Now));
        var sut = new HttpClientAuthenticationHandler(new HttpClientHandler(), new DataHubAuthConfiguration(true, scope), tokenCredentialMock)
        {
            InnerHandler = new TestHandler()
        };
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/");

        var response = await new HttpMessageInvoker(sut).SendAsync(request, new CancellationToken());

        request.Headers.Authorization.ShouldBe(new AuthenticationHeaderValue("Bearer", accessToken));
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
    [Fact]
    public async Task WhenAddAuthorizationHeaderCalled_AndTokenCredentialThrowsException_ThenAuthorizationHeaderIsNull()
    {
        var tokenCredentialMock = Substitute.For<DefaultAzureCredential>();
        tokenCredentialMock.GetTokenAsync(Arg.Any<TokenRequestContext>()).ThrowsAsync(new Exception());
        var sut = new HttpClientAuthenticationHandler(new HttpClientHandler(), new DataHubAuthConfiguration(true, "scope"), tokenCredentialMock)
        {
            InnerHandler = new TestHandler()
        };
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/");

        try
        {
            var response = await new HttpMessageInvoker(sut).SendAsync(request, new CancellationToken());
            response.ShouldBeNull();
        }
        catch (Exception e)
        {
            e.ShouldBeOfType<HttpRequestException>();
            e.Message.ShouldBe("Unable to authenticate with backend service.");
        }
        request.Headers.Authorization.ShouldBeNull();
    }

    [Fact]
    public async Task WhenSendAsyncCalledWithDisabledAuthentication_ThenAuthorizationHeaderIsNull()
    {
        var tokenCredentialMock = Substitute.For<DefaultAzureCredential>();
        var sut = new HttpClientAuthenticationHandler(new HttpClientHandler(), new DataHubAuthConfiguration(false, "scope"), tokenCredentialMock)
        {
            InnerHandler = new TestHandler()
        };
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/");

        var response = await new HttpMessageInvoker(sut).SendAsync(request, new CancellationToken());

        request.Headers.Authorization.ShouldBeNull();
        tokenCredentialMock.ReceivedCalls().ShouldBeEmpty();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WhenSendAsyncRaisesException_ThenAuthorizationHeaderIsNull()
    {
        var tokenCredentialMock = Substitute.For<DefaultAzureCredential>();
        var sut = new HttpClientAuthenticationHandler(new HttpClientHandler(), new DataHubAuthConfiguration(false, "scope"), tokenCredentialMock)
        {
            InnerHandler = new TestHandler()
        };
        tokenCredentialMock.GetTokenAsync(Arg.Any<TokenRequestContext>()).ThrowsAsync(new Exception());
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/");

        var response = await new HttpMessageInvoker(sut).SendAsync(request, new CancellationToken());

        request.Headers.Authorization.ShouldBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    sealed private class TestHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

}