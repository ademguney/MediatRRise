using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MediatRRise.Behaviors.Performance;

/// <summary>
/// Measures execution time of requests and logs if it exceeds a threshold.
/// </summary>
public class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
    int thresholdMs = 500) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request by measuring its execution time and logging if it exceeds the threshold.
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
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > thresholdMs)
        {
            logger.LogWarning(
                "[Performance] {Request} took {Elapsed}ms (threshold: {Threshold}ms)",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds,
                thresholdMs
            );
        }
        else
        {
            logger.LogInformation(
                "[Performance] {Request} completed in {Elapsed}ms",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds
            );
        }

        return response;
    }
}