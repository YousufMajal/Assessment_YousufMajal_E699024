namespace Application.Abstractions;

public class ValidationResultT
{
    public sealed class ValidationResult<TValue> : Result<TValue>, IValidationResult
    {
        private ValidationResult(Error[] errors)
            : base(false, IValidationResult.ValidationError, default) =>
            Errors = errors;

        public Error[] Errors { get; }

        public static ValidationResult<TValue> WithErrors(Error[] errors) => new(errors);
    }
}