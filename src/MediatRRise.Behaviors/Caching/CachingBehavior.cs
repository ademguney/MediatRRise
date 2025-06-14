using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatRRise.Behaviors.Caching;

/// <summary>
/// Caches the response of a request if it implements <see cref="ICacheableRequest{TResponse}"/>.
/// </summary>
public class CachingBehavior<TRequest, TResponse>(
    ICacheService cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICacheableRequest<TResponse> cacheable)
            return await next();

        var cacheKey = cacheable.CacheKey;

        var cached = await cache.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            logger.LogInformation("[Cache] Hit for key: {CacheKey}", cacheKey);
            return cached;
        }

        var response = await next();

        await cache.SetAsync(cacheKey, response, cacheable.Expiration, cancellationToken);
        logger.LogInformation("[Cache] Set for key: {CacheKey}", cacheKey);

        return response;
    }
}