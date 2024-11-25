namespace Catalog.Domain.Primitives
{
    public class Result
    {
        protected Result(bool isSuccess, IEnumerable<string> errors)
        {
            if (isSuccess && errors.Any())
                throw new InvalidOperationException();
            if (!isSuccess && !errors.Any())
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Errors = errors.ToArray();
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string[] Errors { get; }

        public static Result Success() => new Result(true, Array.Empty<string>());
        public static Result Failure(string error) => new Result(false, new[] { error });
        public static Result Failure(IEnumerable<string> errors) => new Result(false, errors);
    }

    public class Result<T> : Result
    {
        private readonly T? _value;

        protected Result(T? value, bool isSuccess, IEnumerable<string> errors) 
            : base(isSuccess, errors)
        {
            _value = value;
        }

        public T Value => IsSuccess 
            ? _value! 
            : throw new InvalidOperationException("Cannot access value of a failed result");

        public static Result<T> Success(T value) => new Result<T>(value, true, Array.Empty<string>());
        public static new Result<T> Failure(string error) => new Result<T>(default, false, new[] { error });
        public static new Result<T> Failure(IEnumerable<string> errors) => new Result<T>(default, false, errors);
    }
} 