namespace MediatRRise.Core.Abstractions;

/// <summary>
///  A market interface for requests that should be cached.
/// </summary>
public interface ICacheableRequest<TResponse> : IRequest<TResponse>
{
    /// <summary>
    /// Unique cache key for the request.
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// Cache duration in seconds.
    /// </summary>
    TimeSpan? Expiration { get; }
}