using FluentValidation;
using MediatRRise.Core.Abstractions;

namespace MediatRRise.Behaviors.Validation;

/// <summary>
///  Pipeline behavior to validate incoming requests before they are handled.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                var errorMessage = string.Join(Environment.NewLine, failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
                throw new ValidationException(errorMessage);
            }
        }

        return await next();
    }
}