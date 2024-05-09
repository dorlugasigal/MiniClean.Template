using Core.Models;
using Core.Validators;

namespace Unit.Tests.Core.Validators;

public class PatientIdValidatorTests
{
    private readonly PatientIdValidator _validator = new();


    [Fact]
    public void GivenEmptyPatientId_ShouldFailValidation()
    {
        var invalidInput = new PatientId(string.Empty);
        var validationResult = _validator.Validate(invalidInput);

        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.ShouldContain(error => error.ErrorMessage == "PatientId cannot be empty");
    }


    [Fact]
    public void GivenNullPatientId_ShouldFailValidation()
    {
        var invalidInput = new PatientId(null);
        var validationResult = _validator.Validate(invalidInput);

        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.ShouldContain(error => error.ErrorMessage == "PatientId cannot be null");
    }

    [Fact]
    public void GivenValidPatientId_ShouldPassValidation()
    {
        var validInput = new PatientId("1234567890");
        var validationResult = _validator.Validate(validInput);

        validationResult.IsValid.ShouldBeTrue();
        validationResult.Errors.ShouldBeEmpty();
    }
}