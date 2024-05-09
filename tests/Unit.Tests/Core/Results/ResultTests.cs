using Core.Results;

namespace Unit.Tests.Core.Results;

public class ResultTests
{
    #region Result<T> tests

    [Fact]
    public void GivenValueIsNotNull_WhenCheckingIsSuccess_ThenReturnsTrue()
    {
        var result = new Result<string>("test");
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("test");
    }

    [Fact]
    public void GivenExceptionIsNotNull_WhenCheckingIsFailure_ThenReturnsTrue()
    {
        var expectedException = new Exception("test exception");
        var result = new Result<string>(expectedException);
        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBe(expectedException);
    }

    [Fact]
    public void GivenValueAndExceptionAreNull_WhenCheckingIsNull_ThenReturnsTrue()
    {
        var result = new Result<string>();
        result.IsNull.ShouldBeTrue();
    }

    [Fact]
    public void GivenResultState_WhenCallingMatch_ThenCallsCorrectFuncBasedOnResultState()
    {
        var successResult = new Result<string>("test");
        var failureResult = new Result<string>(new Exception());
        var nullResult = new Result<string>();

        successResult.Match(value => value, _ => "failure", () => "null").ShouldBe("test");
        failureResult.Match(value => value, _ => "failure", () => "null").ShouldBe("failure");
        nullResult.Match(value => value, _ => "failure", () => "null").ShouldBe("null");
    }

    [Fact]
    public void GivenNullResultAndNoOnNullFunc_WhenCallingMatch_ThenThrowsInvalidOperationException()
    {
        var nullResult = new Result<string>();
        var exception = Should.Throw<InvalidOperationException>(() => nullResult.Match(value => value, _ => "failure"));
        exception.Message.ShouldBe("Result is null, but no onNull function was provided.");
    }

    [Fact]
    public void GivenValue_WhenUsingImplicitOperatorFromValue_ThenReturnsSuccessResult()
    {
        Result<string> result = "test";
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("test");
    }

    [Fact]
    public void GivenNull_WhenUsingImplicitOperatorFromNull_ThenReturnsNullResult()
    {
        Result<string> result = default(string);
        result.IsNull.ShouldBeTrue();
    }

    [Fact]
    public void GivenException_WhenUsingImplicitOperatorFromException_ThenReturnsFailureResult()
    {
        var exception = new Exception("test exception");
        Result<string> result = exception;
        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBe(exception);
    }

    #endregion

    #region Result tests

    [Fact]
    public void GivenNothing_WhenCallingSuccess_ThenSetsIsSuccessToTrue()
    {
        var result = Result.Success();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void GivenException_WhenCallingFailure_ThenSetsIsFailureToTrueAndSetsException()
    {
        var exception = new Exception();
        var result = Result.Failure(exception);

        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBe(exception);
    }

    [Fact]
    public void GivenException_WhenUsingImplicitConversionFromException_ThenSetsIsFailureToTrueAndSetsException()
    {
        var exception = new Exception();
        Result result = exception;

        result.IsFailure.ShouldBeTrue();
        result.Exception.ShouldBe(exception);
    }

    [Fact]
    public void GivenResultState_WhenCallingMatch_ThenCallsCorrectFunctionBasedOnResultState()
    {
        var successResult = Result.Success();
        var failureResult = Result.Failure(new Exception());

        successResult.Match(() => "success", _ => "failure").ShouldBe("success");
        failureResult.Match(() => "success", _ => "failure").ShouldBe("failure");
    }

    [Fact]
    public void GivenSuccessResult_WhenUsingImplicitConversionFromResult_ThenSetsIsSuccessToTrue()
    {
        var successResult = new Result<int>(1);
        Result result = successResult;

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Exception.ShouldBeNull();
    }

    [Fact]
    public void GivenFailureResult_WhenUsingImplicitConversionFromResult_ThenSetsIsFailureToTrueAndSetsException()
    {
        var exception = new Exception();
        var failureResult = new Result<int>(exception);
        Result result = failureResult;

        result.IsFailure.ShouldBeTrue();
        result.IsSuccess.ShouldBeFalse();
        result.Exception.ShouldBe(exception);
    }

    #endregion
}
