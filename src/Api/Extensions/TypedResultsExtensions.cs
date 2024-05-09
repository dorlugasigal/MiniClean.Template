using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Extensions;

public static class ValidationResultExtensions
{
    public static BadRequest<List<string>> ToBadRequest(this ValidationResult validationResult)
    {
        return TypedResults.BadRequest(
            validationResult.Errors
                .Select(error => error.ErrorMessage)
                .ToList()
        );
    }
}