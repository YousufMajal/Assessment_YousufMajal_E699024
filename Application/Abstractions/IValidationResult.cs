using Application.Errors;

namespace Application.Abstractions
{
    public interface IValidationResult
    {
        public static readonly Error ValidationError = ValidationErrorResponses.ValidationError();

        Error[] Errors { get; }
    }
}