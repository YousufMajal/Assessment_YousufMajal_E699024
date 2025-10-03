using Application.Abstractions;

namespace Application.Errors;

public class ValidationErrorResponses
{
    public static Error ValidationError() => new(
            "Validation.Error",
            "Validation error/'s occurred"
        );
}