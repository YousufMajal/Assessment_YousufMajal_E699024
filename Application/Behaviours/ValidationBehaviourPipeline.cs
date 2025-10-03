using Application.Abstractions;
using FluentValidation;
using MediatR;
using static Application.Abstractions.ValidationResultT;

namespace Application.Behaviours
{
    public sealed class ValidationBehaviourPipeline<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
        where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviourPipeline(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var errors = _validators
               .Select(validationResult => validationResult.Validate(request))
               .SelectMany(validationResult => validationResult.Errors)
               .Where(validationFailure => validationFailure is not null)
               .Select(validationFailure => new Error(
                   validationFailure.PropertyName,
                   validationFailure.ErrorMessage))
               .Distinct()
               .ToArray();

            var context = new ValidationContext<TRequest>(request);

            var validationFailures = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context)));

            if (errors.Any())
            {
                return CreateValidationResult<TResponse>(errors);
            }

            return await next();
        }

        private static TResult CreateValidationResult<TResult>(Error[] errors)
            where TResult : Result
        {
            if (typeof(TResult) == typeof(Result))
            {
                return (ValidationResult.WithErrors(errors) as TResult)!;
            }

            object validationResult = typeof(ValidationResult<>)
                .GetGenericTypeDefinition()
                .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
                .GetMethod(nameof(ValidationResult.WithErrors))!
                .Invoke(null, new object[] { errors })!;

            return (TResult)validationResult;
        }
    }
}