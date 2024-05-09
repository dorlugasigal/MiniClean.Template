using Core.Results;
using Hl7.Fhir.Model;

namespace Core.Abstractions.Services;

public interface IPatientsService
{
    Task<Result<Patient>> GetById(string id);
}