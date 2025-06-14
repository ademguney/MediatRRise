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