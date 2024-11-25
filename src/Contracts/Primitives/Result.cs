namespace Contracts.Primitives;

public class Result<T>
{
    private readonly List<string> _errors = new();
    
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public IReadOnlyList<string> Errors => _errors.AsReadOnly();

    private Result(T value, bool isSuccess, IEnumerable<string> errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        _errors.AddRange(errors);
    }

    public static Result<T> Success(T value) => 
        new(value, true, Array.Empty<string>());

    public static Result<T> Failure(IEnumerable<string> errors) => 
        new(default!, false, errors);

    public static Result<T> Failure(string error) => 
        new(default!, false, new[] { error });
}

public class Result
{
    private readonly List<string> _errors = new();
    
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<string> Errors => _errors.AsReadOnly();

    protected Result(bool isSuccess, IEnumerable<string> errors)
    {
        IsSuccess = isSuccess;
        _errors.AddRange(errors);
    }

    public static Result Success() => 
        new(true, Array.Empty<string>());

    public static Result Failure(IEnumerable<string> errors) => 
        new(false, errors);

    public static Result Failure(string error) => 
        new(false, new[] { error });

    public static Result<T> Success<T>(T value) => 
        Result<T>.Success(value);

    public static Result<T> Failure<T>(IEnumerable<string> errors) => 
        Result<T>.Failure(errors);

    public static Result<T> Failure<T>(string error) => 
        Result<T>.Failure(error);
} 