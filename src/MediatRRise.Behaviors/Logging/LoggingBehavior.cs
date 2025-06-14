using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatRRise.Behaviors.Logging;

/// <summary>
/// Logs before and after a request is handled.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
{
    /// <summary>
    /// Handles the request by logging before and after the next step in the pipeline is executed.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("[Logging] Handling request: {Request}", typeof(TRequest).Name);

        var response = await next();

        logger.LogInformation("[Logging] Handled request: {Request}", typeof(TRequest).Name);

        return response;
    }
}