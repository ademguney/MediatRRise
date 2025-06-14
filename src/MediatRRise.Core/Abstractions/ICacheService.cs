namespace MediatRRise.Core.Abstractions;

/// <summary>
/// Provides cache access abstraction.
/// </summary>
public interface ICacheService
{
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
}