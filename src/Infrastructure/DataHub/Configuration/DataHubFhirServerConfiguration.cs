namespace Infrastructure.DataHub.Configuration;

public record DataHubFhirServerConfiguration(string BaseUrl, DataHubAuthConfiguration Authentication)
{
    public const string SectionKey = "DataHubFhirServer";
}

public record DataHubAuthConfiguration(bool IsEnabled, string Scope);