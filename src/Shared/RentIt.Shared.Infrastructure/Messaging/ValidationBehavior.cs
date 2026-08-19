using System.Reflection;
using FluentValidation;
using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Shared.Infrastructure.Messaging;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationFailures
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => validationFailure.ErrorMessage)
            .Distinct()
            .ToArray();

        if (errors.Length != 0)
        {
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Result<>)
                    .MakeGenericType(resultType)
                    .GetMethod("Failure", BindingFlags.Public | BindingFlags.Static);

                if (failureMethod != null)
                {
                    return (TResponse)failureMethod.Invoke(null, [new Error("ValidationFailed", string.Join(", ", errors))])!;
                }
            }
            else if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)Result.Failure(new Error("ValidationFailed", string.Join(", ", errors)));
            }

            throw new ValidationException(string.Join(", ", errors));
        }

        return await next(cancellationToken);
    }
}
