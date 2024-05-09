using System.Net;
using Core.Abstractions.Clients;
using Core.Services;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Unit.Tests.Core.Services;

public class PatientsServiceTests
{
    private readonly IDataHubFhirClient _fhirClient;
    private readonly PatientsService _sut;

    public PatientsServiceTests()
    {
        var logger = Substitute.For<ILogger<PatientsService>>();
        _fhirClient = Substitute.For<IDataHubFhirClient>();
        _sut = new PatientsService(logger, _fhirClient);
    }

    [Fact]
    public async Task GivenValidPatientId_WhenPatientExistsInDataHub_ShouldReturnPatient()
    {
        const string patientId = "9730524319";
        var existingPatient = new Patient { Id = patientId };
        _fhirClient.GetResource<Patient>(patientId).Returns(existingPatient);

        var result = await _sut.GetById(patientId);

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(existingPatient);
        result.Exception.ShouldBeNull();
    }

    [Fact]
    public async Task GivenValidNhsNumber_WhenPatientDoesNotExistInDataHubAndPds_ShouldReturnFailure()
    {
        const string patientId = "9730524319";
        _fhirClient.GetResource<Patient>(patientId).Returns(new FhirOperationException("Not found", HttpStatusCode.NotFound));

        var result = await _sut.GetById(patientId);

        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBeOfType<FhirOperationException>();
        result.Exception.Message.ShouldBe("Not found");
        var exception = result.Exception as FhirOperationException;
        exception?.Status.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GivenValidNhsNumber_WhenErrorOccursWhileFetchingPatientFromDataHub_ShouldReturnFailure()
    {
        const string patientId = "9730524319";
        _fhirClient.GetResource<Patient>(patientId).Returns(new Exception("error"));

        var result = await _sut.GetById(patientId);

        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBeOfType<ApplicationException>();
    }
}