using Core.Abstractions.Clients;
using Hl7.Fhir.Model;
using Microsoft.Extensions.DependencyInjection;
using Task = System.Threading.Tasks.Task;

namespace Integration.Tests.DataHub.Clients;

public class DataHubFhirClientTests : IDisposable
{
    private readonly ApiWebApplicationFactory _webApplicationFactory;
    private readonly IDataHubFhirClient _fhirClient;

    public DataHubFhirClientTests()
    {
        _webApplicationFactory = new ApiWebApplicationFactory();
        _fhirClient = _webApplicationFactory.Services.GetService<IDataHubFhirClient>()
                      ?? throw new Exception("Failed to resolve IDataHubFhirClient from the service provider");
    }

    public void Dispose()
    {
        _webApplicationFactory.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GivenBundleIsNull_ThenTransactionResultIsFailure()
    {
        var actualOperationOutcome = await _fhirClient.TransactionAsync<Bundle>(null!);
        actualOperationOutcome.IsFailure.ShouldBeTrue();
        actualOperationOutcome.Exception.ShouldBeOfType<ArgumentNullException>();
        actualOperationOutcome.Value.ShouldBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GivenIdentifierIsNullOrWhitespace_ThenSearchResultIsFailure(string? identifier)
    {
        var actualOperationOutcome = await _fhirClient.SearchResourceByIdentifier<Patient>(identifier!);
        actualOperationOutcome.IsFailure.ShouldBeTrue();
        actualOperationOutcome.Exception.ShouldBeOfType<ArgumentNullException>();
        actualOperationOutcome.Value.ShouldBeNull();
    }
}