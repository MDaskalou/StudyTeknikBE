using System;

namespace Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }

        // Konstruktorn är protected så man måste använda Success/Failure-metoderna
        protected Result(bool isSuccess, string error)
        {
            // Enkel validering så man inte skapar ogiltiga resultat
            if (isSuccess && error != string.Empty)
                throw new InvalidOperationException("Ett lyckat resultat kan inte ha ett felmeddelande.");
            if (!isSuccess && error == string.Empty)
                throw new InvalidOperationException("Ett misslyckat resultat måste ha ett felmeddelande.");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, string.Empty);
        public static Result Failure(string error) => new(false, error);
        
        public static Result<T> Success<T>(T value) => Result<T>.Success(value);

       
    }

    // Denna används när du vill returnera data, t.ex. Result<Course>
    public class Result<T> : Result
    {
        public T Value { get; }

        protected Result(T value, bool isSuccess, string error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(value, true, string.Empty);
        
        // "new" nyckelordet döljer bas-metoden så vi får rätt returtyp
        public static new Result<T> Failure(string error) => new(default!, false, error);
    }
}