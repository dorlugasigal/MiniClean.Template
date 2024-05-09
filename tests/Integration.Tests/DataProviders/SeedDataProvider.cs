using Core.Abstractions.Clients;
using Hl7.Fhir.Model;

namespace Integration.Tests.DataProviders;

public static class SeedDataProvider
{
    private static Patient Patient { get; } = GetPatient();

    private static Patient GetPatient()
    {
        return new Patient
        {
            Id = "1234567890",
            Active = true,
            Name =
            [
                new HumanName { Use = HumanName.NameUse.Official, Given = new[] { "Bruce" }, Family = "Wayne" }
            ],
            Address =
            [
                new Address { City = "Gotham", Country = "DC-Comic-Land" }
            ]
        };
    }

    public static void RegisterSeedData(IDataHubFhirClient dataHubFhirClient)
    {
        dataHubFhirClient.UpdateResource(Patient).Wait();
    }
}