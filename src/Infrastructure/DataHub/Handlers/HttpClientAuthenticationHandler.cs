using System.Net.Http.Headers;
using Azure.Core;
using Azure.Identity;
using Infrastructure.DataHub.Configuration;

namespace Infrastructure.DataHub.Handlers;

public class HttpClientAuthenticationHandler(HttpClientHandler httpClientHandler, DataHubAuthConfiguration authentication, TokenCredential credential) : DelegatingHandler(httpClientHandler)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (authentication.IsEnabled)
        {
            await AddAuthenticationHeader(request, cancellationToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task AddAuthenticationHeader(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var tokenRequestContext = new TokenRequestContext([authentication.Scope]);
            var token = await credential.GetTokenAsync(tokenRequestContext, cancellationToken);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        }
        catch (Exception)
        {
            throw new HttpRequestException("Unable to authenticate with backend service.");
        }
    }
}