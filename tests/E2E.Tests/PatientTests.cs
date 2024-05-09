namespace E2E.Tests;

public class PatientTests(ITestOutputHelper outputHelper) : BaseApiTest(outputHelper)
{
    [Fact]
    public void PatientSearch_CalledWithInvalidPatientId_ReturnNotFound()
    {
        const string patientId = "1313131313";
        var getRequest = Get($"/Patient/{patientId}");
        var response = ApiClient.Execute(getRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}