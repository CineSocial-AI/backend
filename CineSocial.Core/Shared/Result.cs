namespace CineSocial.Core.Shared;

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public List<string> Errors { get; }

    protected Result(bool isSuccess, string error, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? new List<string>();
    }

    public static Result Success() => new(true, string.Empty);

    public static Result Failure(string error) => new(false, error);

    public static Result Failure(List<string> errors) => new(false, string.Empty, errors);

    public static Result<T> Success<T>(T data) => Result<T>.Success(data);

    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);

    public static Result<T> Failure<T>(List<string> errors) => Result<T>.Failure(errors);
}

public class Result<T> : Result
{
    public T? Data { get; }

    protected Result(bool isSuccess, T? data, string error, List<string>? errors = null)
        : base(isSuccess, error, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => new(true, data, string.Empty);

    public static new Result<T> Failure(string error) => new(false, default, error);

    public static new Result<T> Failure(List<string> errors) => new(false, default, string.Empty, errors);

    public static implicit operator Result<T>(T data) => Success(data);
}