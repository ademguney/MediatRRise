using MediatRRise.Core.Abstractions;

namespace MediatRRise.Behaviors.Retry;

/// <summary>
/// Pipeline behavior that retries failed requests due to transient errors.
/// </summary>
public class RetryBehavior<TRequest, TResponse>(int retryCount = 3, int delayMilliseconds = 200) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly TimeSpan _retryDelay = TimeSpan.FromMilliseconds(delayMilliseconds);
    private readonly Type[] _transientExceptions =
    [
        typeof(TimeoutException),
        typeof(HttpRequestException)
    ];

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        int attempt = 0;

        while (true)
        {
            try
            {
                return await next();
            }
            catch (Exception ex) when (IsTransient(ex))
            {
                attempt++;

                if (attempt >= retryCount)
                    throw;

                await Task.Delay(_retryDelay, cancellationToken);
            }
        }
    }

    private bool IsTransient(Exception ex) => _transientExceptions.Any(t => t.IsAssignableFrom(ex.GetType()));
}