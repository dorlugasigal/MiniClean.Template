using System.Net;
using Api.Extensions;
using Carter;
using Core.Abstractions.Services;
using Core.Models;
using FluentValidation;
using Hl7.Fhir.Rest;

namespace Api.Modules;

public class PatientModule : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var patientsGroup = app.MapGroup("Patient");
        patientsGroup.MapGet("/{id}", GetPatientById).WithName("GetPatientById");
    }

    private static async Task<IResult> GetPatientById(string id, IValidator<PatientId> validator, IPatientsService patientsService, ILogger<PatientModule> logger)
    {
        var validationResult = await validator.ValidateAsync(id);
        if (!validationResult.IsValid)
        {
            return validationResult.ToBadRequest();
        }

        var getByIdResult = await patientsService.GetById(id);

        return getByIdResult.Match(
            onSuccess: TypedResults.Ok,
            onFailure: MapExceptionToResult
        );
    }

    private static IResult MapExceptionToResult(Exception exception)
    {
        return exception switch
        {
            ApplicationException => TypedResults.BadRequest(exception),
            FhirOperationException { Status: HttpStatusCode.NotFound } => TypedResults.NotFound(),
            _ => throw exception
        };
    }
}