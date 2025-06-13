using MediatRRise.Core.Abstractions;

namespace MediatRRise.Behaviors.Logging;

/// <summary>
/// Logs before and after a request is handled.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Logging] Handling request: {typeof(TRequest).Name}");

        var response = await next();

        Console.WriteLine($"[Logging] Handled request: {typeof(TRequest).Name}");

        return response;
    }
}