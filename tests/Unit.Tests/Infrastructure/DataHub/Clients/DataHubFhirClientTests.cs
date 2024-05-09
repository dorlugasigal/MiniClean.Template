using System.Net;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Infrastructure.DataHub.Clients;
using Infrastructure.DataHub.Clients.Abstractions;
using Infrastructure.DataHub.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using Unit.Tests.Infrastructure.Common.HealthCheck;
using Id = Hl7.Fhir.Model.Id;
using Task = System.Threading.Tasks.Task;


namespace Unit.Tests.Infrastructure.DataHub.Clients;

public class DataHubFhirClientTests
{
    private readonly DataHubFhirClient _dataHubFhirClient;
    private readonly IDataHubFhirClientWrapper _fhirClientMock;
    private readonly IHttpClientFactory? _clientFactoryMock;
    private readonly ILogger<DataHubFhirClient> _loggerMock;


    public DataHubFhirClientTests()
    {
        _fhirClientMock = Substitute.For<IDataHubFhirClientWrapper>();
        _clientFactoryMock = Substitute.For<IHttpClientFactory>();
        _loggerMock = Substitute.For<ILogger<DataHubFhirClient>>();
        _dataHubFhirClient = new DataHubFhirClient(_loggerMock, _fhirClientMock);
    }

    [Fact]
    public async Task CreateResource_CreatesResourceSuccessfully()
    {
        var patient = new Patient();
        _fhirClientMock.CreateResource(patient).Returns(patient);

        var result = await _dataHubFhirClient.CreateResource(patient);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(patient);
        await _fhirClientMock.Received().CreateResource(Arg.Is<Patient>(p => p.IdElement == null));
    }

    [Fact]
    public async Task CreateResource_ReturnsFailureWithException_WhenExceptionThrownFromClient()
    {
        var patient = new Patient { IdElement = new Id("123") };
        var exception = new Exception("Error creating resource");
        _fhirClientMock.CreateResource(patient).Throws(exception);

        var result = await _dataHubFhirClient.CreateResource(patient);
        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBe(exception);
    }

    [Fact]
    public async Task GetResource_ShouldReturnResource_WhenResourceExists()
    {
        const string resourceId = "123";
        var existingResource = new Patient { IdElement = new Id(resourceId) };
        _fhirClientMock.ReadAsync<Patient>($"{nameof(Patient)}/{resourceId}").Returns(existingResource);

        var result = await _dataHubFhirClient.GetResource<Patient>(resourceId);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(existingResource);
    }

    [Fact]
    public async Task GetResource_ShouldThrowException_WhenExceptionIsThrown()
    {
        const string resourceId = "123";
        _fhirClientMock.ReadAsync<Patient>($"{nameof(Patient)}/{resourceId}")
            .Throws(new Exception("Error Getting Resource"));

        var result = await _dataHubFhirClient.GetResource<Patient>(resourceId);
        result.IsFailure.ShouldBeTrue();
        result.Exception?.Message.ShouldBe("Error Getting Resource");
    }

    [Fact]
    public async Task GetResource_ReturnsResource_WhenResourceExists()
    {
        const string resourceId = "test-resource-id";
        var expectedResource = new Patient();
        _fhirClientMock.ReadAsync<Patient>(Arg.Any<string>()).Returns(expectedResource);

        var result = await _dataHubFhirClient.GetResource<Patient>(resourceId);
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedResource);
    }

    [Fact]
    public async Task GetResource_ShouldReturnFhirOperationException_WhenResourceDoesNotExist()
    {
        const string resourceId = "123";
        _fhirClientMock.ReadAsync<Patient>($"{nameof(Patient)}/{resourceId}")
            .Throws(new FhirOperationException("Not Found", HttpStatusCode.NotFound));

        var result = await _dataHubFhirClient.GetResource<Patient>(resourceId);

        result.IsFailure.ShouldBeTrue();
        result.Exception?.Message.ShouldBe("Not Found");
    }


    [Fact]
    public async Task UpdateResource_ShouldReturnResource_WhenUpdateSucceeds()
    {
        var resource = new Patient { Id = "456" };
        _fhirClientMock.UpdateAsync(resource).Returns(resource);

        var result = await _dataHubFhirClient.UpdateResource(resource);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(resource);
    }

    [Fact]
    public async Task UpdateResource_ShouldReturnFailureWithException_WhenUpdateFails()
    {
        var resource = new Patient { Id = "456" };
        _fhirClientMock.UpdateAsync(resource).Throws(new Exception("Error Updating"));

        var result = await _dataHubFhirClient.UpdateResource(resource);

        result.IsFailure.ShouldBeTrue();
        result.Exception.Message.ShouldBe("Error Updating");
    }

    [Fact]
    public async Task UpdateResource_ShouldReturnFailureWithNullArgException_WhenResourceIsNull()
    {
        var result = await _dataHubFhirClient.UpdateResource<Patient>(null!);

        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBeOfType<ArgumentNullException>();
    }

    [Fact]
    public async Task SearchResourceByIdentifier_ShouldReturnResource_WhenResourceExists()
    {
        const string identifier = "123";
        var existingResource = new Patient { Id = identifier };
        _fhirClientMock.SearchResourceByIdentifier<Patient>(identifier)
            .Returns(new Bundle { Entry = [new Bundle.EntryComponent { Resource = existingResource }] });

        var result = await _dataHubFhirClient.SearchResourceByIdentifier<Patient>(identifier);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(existingResource);
    }

    [Fact]
    public async Task SearchResourceByIdentifier_ShouldReturnFailureWithNotFoundException_WhenResourceDoesNotExist()
    {
        const string identifier = "123";
        _fhirClientMock.SearchResourceByIdentifier<Patient>(identifier)
            .Returns(new Bundle { Entry = new List<Bundle.EntryComponent>() });

        var result = await _dataHubFhirClient.SearchResourceByIdentifier<Patient>(identifier);

        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBeOfType<FhirOperationException>();
        var asFhirOperationException = (FhirOperationException)result.Exception;
        asFhirOperationException.Status.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchResourceByIdentifier_ShouldReturnFailureWithException_WhenExceptionIsThrown()
    {
        const string identifier = "123";
        var expectedException = new Exception("Error Searching for resource");
        _fhirClientMock.SearchResourceByIdentifier<Patient>(identifier).Throws(expectedException);

        var result = await _dataHubFhirClient.SearchResourceByIdentifier<Patient>(identifier);

        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBe(expectedException);
    }

    [Fact]
    public async Task SearchResourceByParams_ReturnsBundle_WhenResourcesFound()
    {
        var searchParams = new SearchParams().Where("name=test");
        var expectedBundle = new Bundle { Entry = [new Bundle.EntryComponent { Resource = new Patient() }] };
        _fhirClientMock.SearchResourceByParams<Patient>(searchParams).Returns(expectedBundle);

        var result = await _dataHubFhirClient.SearchResourceByParams<Patient>(searchParams);

        result.ShouldBe(expectedBundle);
    }

    [Fact]
    public async Task SearchResourceByParams_ShouldReturnFailureWithException_WhenExceptionIsThrown()
    {
        var searchParams = new SearchParams().Where("name=test");
        var expectedException = new Exception("Error Searching for resource");
        _fhirClientMock.SearchResourceByParams<Patient>(searchParams).Throws(expectedException);

        var result = await _dataHubFhirClient.SearchResourceByParams<Patient>(searchParams);

        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBe(expectedException);
    }

    [Fact]
    public async Task SearchResourceByParams_ReturnsFailureWithFhirOperationException_WhenBadRequest()
    {
        _fhirClientMock.SearchResourceByParams<Patient>(Arg.Any<SearchParams>())
            .Throws(new FhirOperationException("Bad Request Message", HttpStatusCode.BadRequest));

        var result = await _dataHubFhirClient.SearchResourceByParams<Patient>(new SearchParams());

        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBeOfType<FhirOperationException>();
        var asFhirOperationException = (FhirOperationException)result.Exception;
        asFhirOperationException.Status.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchResourceByParams_ReturnsFailureWithFhirOperationException_WhenResourceIsNotFound()
    {
        var bundle = new Bundle { Entry = [] };
        _fhirClientMock.SearchResourceByParams<Patient>(Arg.Any<SearchParams>()).Returns(bundle);

        var result = await _dataHubFhirClient.SearchResourceByParams<Patient>(new SearchParams());

        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBeOfType<FhirOperationException>();
        var asFhirOperationException = (FhirOperationException)result.Exception;
        asFhirOperationException.Status.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchResourceByParams_ReturnsBundleAllResources_WhenSearchWithEmptyParams()
    {
        var searchParams = new SearchParams();
        var expectedBundle = new Bundle { Entry = [new Bundle.EntryComponent { Resource = new Patient() }] };
        _fhirClientMock.SearchResourceByParams<Patient>(searchParams).Returns(expectedBundle);

        var result = await _dataHubFhirClient.SearchResourceByParams<Patient>(searchParams);

        result.ShouldBe(expectedBundle);
    }

    [Fact]
    public async Task ContinueAsync_WhenNextLinkExists_ShouldReturnNextPage()
    {
        var bundle = new Bundle { Entry = [] };
        var expectedBundle = new Bundle { Entry = [new Bundle.EntryComponent { Resource = new Patient() }] };
        _fhirClientMock.ContinueAsync(bundle).Returns(expectedBundle);

        var result = await _dataHubFhirClient.ContinueAsync(bundle);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedBundle);
    }

    [Fact]
    public async Task ContinueAsync_WhenNextLinkDoesNotExist_ShouldReturnNull()
    {
        var bundle = new Bundle { Entry = [] };
        Bundle expectedBundle = null!;

        _fhirClientMock.ContinueAsync(bundle).Returns(expectedBundle);

        var result = await _dataHubFhirClient.ContinueAsync(bundle);

        result.IsNull.ShouldBeTrue();
    }

    [Fact]
    public async Task ContinueAsync_WhenExceptionIsThrown_ShouldThrow()
    {
        var bundle = new Bundle { Entry = [] };
        _fhirClientMock.ContinueAsync(Arg.Any<Bundle>()).Throws(new Exception("test"));

        var ex = await _dataHubFhirClient.ContinueAsync(bundle).ShouldThrowAsync<Exception>();

        ex.Message.ShouldBe("test");
    }

    private async Task SetupHttpClientMock(Resource resource, HttpStatusCode statusCode = HttpStatusCode.OK, Boolean exceptionThrown = false)
    {
        var serializer = new FhirJsonSerializer();
        var jsonString = await serializer.SerializeToStringAsync(resource);
        if (exceptionThrown)
        {
            _clientFactoryMock!.CreateClient(Arg.Any<string>()).Throws(new Exception("some-exception"));
            return;
        }

        HttpClientMocker.SetupHttpClient(
            _clientFactoryMock ?? throw new InvalidOperationException(),
            statusCode,
            jsonString);
    }
}