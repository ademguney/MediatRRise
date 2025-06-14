using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatRRise.Behaviors.ExceptionHandling;

/// <summary>
///  Catches and logs unhandled exceptions during request handling.
/// </summary>
public class ExceptionHandlingBehavior<TRequest, TResponse>(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            logger.LogError(ex, "[Exception] An unhandled exception occurred while processing request: {RequestName}", requestName);

            throw;
        }
    }
}