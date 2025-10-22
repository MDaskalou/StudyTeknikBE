namespace Application.Common.Results
{
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        InternalServiceError = 4,
        Forbidden = 5
    }

    public record Error(string Code, string Description, ErrorType Type)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
        public static Error NotFound(string code, string desc) => new(code, desc, ErrorType.NotFound);
        public static Error Conflict(string code, string desc) => new(code, desc, ErrorType.Conflict);
        public static Error Validation(string code, string desc) => new(code, desc, ErrorType.Validation);
        public static Error InternalServiceError(string code, string desc) => new(code, desc, ErrorType.InternalServiceError);
        public static Error Forbidden(string code, string desc) => new(code, desc, ErrorType.Failure);
    }

    public class OperationResult
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        protected OperationResult(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        // Dessa metoder returnerar BARA icke-generiska resultat
        public static OperationResult Success() => new(true, Error.None);
        public static OperationResult Failure(Error error) => new(false, error);
    }

    public class OperationResult<T> : OperationResult
    {
        public T? Value { get; }

        protected internal OperationResult(T? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        // Dessa metoder returnerar BARA generiska resultat
        public static OperationResult<T> Success(T value) => new(value, true, Error.None);
        public static OperationResult<T> Created(T value) => new(value, true, Error.None);
        public static new OperationResult<T> Failure(Error error) => new(default, false, error);
    }
}