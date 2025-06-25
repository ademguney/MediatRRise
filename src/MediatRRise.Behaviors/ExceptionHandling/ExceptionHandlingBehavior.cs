using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatRRise.Behaviors.ExceptionHandling;

/// <summary>
///  Catches and logs unhandled exceptions during request handling.
/// </summary>
public class ExceptionHandlingBehavior<TRequest, TResponse>(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
{
    /// <summary>
    /// Handles the request and catches any unhandled exceptions that occur during processing.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (FluentValidation.ValidationException ex)
        {
            var requestName = typeof(TRequest).Name;
            var errorMessages = string.Join(" | ", ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            logger.LogWarning(ex, "[ValidationException] Validation failed for {RequestName}: {Errors}", requestName, errorMessages);

            throw new InvalidOperationException($"Validation failed: {errorMessages}");
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            logger.LogError(ex, "[Exception] An unhandled exception occurred while processing request: {RequestName}", requestName);

            throw;
        }
    }
}