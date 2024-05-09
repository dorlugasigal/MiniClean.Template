using System.Diagnostics.Contracts;

namespace Core.Results;

public readonly struct Result<T>
{
    private enum ResultState
    {
        Null,
        Failure,
        Success
    }

    private readonly ResultState _state;

    public T Value { get; }
    public Exception Exception { get; }

    public bool IsSuccess => _state == ResultState.Success;
    public bool IsFailure => _state == ResultState.Failure;
    public bool IsNull => _state == ResultState.Null;

    public Result(T value)
    {
        Value = value;
        Exception = null!;
        _state = ResultState.Success;
    }

    public Result(Exception exception)
    {
        Value = default!;
        Exception = exception;
        _state = ResultState.Failure;
    }

    [Pure]
    public TR Match<TR>(Func<T, TR> onSuccess, Func<Exception, TR> onFailure, Func<TR>? onNull = null) =>
        IsSuccess ? onSuccess(Value) :
        IsFailure ? onFailure(Exception) :
        onNull is not null 
            ? onNull() 
            : throw new InvalidOperationException("Result is null, but no onNull function was provided.");

    public static implicit operator Result<T>(T? value) => value is not null ? new Result<T>(value) : new Result<T>();
    public static implicit operator Result<T>(Exception exception) => new(exception);
    public static implicit operator Result(Result<T> result) => result.IsSuccess ? Result.Success() : Result.Failure(result.Exception);
}

public readonly struct Result(Exception exception)
{
    public Exception Exception { get; } = exception;

    public bool IsSuccess => Exception is null;
    public bool IsFailure => Exception is not null;

    [Pure]
    public TR Match<TR>(Func<TR> onSuccess, Func<Exception, TR> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Exception);

    public static implicit operator Result(Exception exception) => new(exception);
    public static Result Success() => new();
    public static Result Failure(Exception exception) => new(exception);
}
