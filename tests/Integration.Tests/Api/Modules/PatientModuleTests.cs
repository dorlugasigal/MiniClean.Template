using System.Net;
using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Task = System.Threading.Tasks.Task;

namespace Integration.Tests.Api.Modules;

public class PatientModuleTests : IDisposable
{
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions().ForFhir(ModelInfo.ModelInspector);
    private readonly ApiWebApplicationFactory _webApplicationFactory;

    public PatientModuleTests()
    {
        _webApplicationFactory = new ApiWebApplicationFactory();
        _client = _webApplicationFactory.CreateClient();
    }

    public void Dispose()
    {
        _webApplicationFactory.Dispose();
        _client.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task WhenGetPatient_WithInvalidPatientId_ThenReturnsBadRequest()
    {
        var response = await _client.GetAsync("/patient/%20");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenGetPatient_WithNonExistingPatientId_ThenReturnsNotFound()
    {
        var response = await _client.GetAsync("/patient/8759655151");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenGetPatient_WithValidPatientId_ThenReturnsPatient()
    {
        var response = await _client.GetAsync("/patient/1234567890");

        response.EnsureSuccessStatusCode();

        var patient = await response.Content.ReadFromJsonAsync<Patient>(_jsonSerializerOptions);
        patient.ShouldNotBeNull();
        patient.Name.First().Family.ShouldBe("Wayne");
        patient.Address.First().City.ShouldBe("Gotham");
        patient.Address.First().Country.ShouldBe("DC-Comic-Land");
    }
}