using Core.Models;
using FluentValidation;

namespace Core.Validators;

public class PatientIdValidator : AbstractValidator<PatientId>
{
    public PatientIdValidator()
    {
        RuleFor(x => x.Value)
            .NotNull()
            .WithMessage("PatientId cannot be null")
            .NotEmpty()
            .WithMessage("PatientId cannot be empty");
    }

}