namespace KodiakPlugBank.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public int? StatusCode { get; }

    protected Result(bool isSuccess, string? error, int? statusCode = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
    }

    public static Result Ok() => new(true, null);
    public static Result Fail(string error, int statusCode = 400) => new(false, error, statusCode);

    public static Result<T> Ok<T>(T value) => new(value, true, null, null);
    public static Result<T> Fail<T>(string error, int statusCode = 400) => new(default, false, error, statusCode);
}

public class Result<T> : Result
{
    public T? Value { get; }

    public Result(T? value, bool isSuccess, string? error, int? statusCode)
        : base(isSuccess, error, statusCode)
    {
        Value = value;
    }
}
