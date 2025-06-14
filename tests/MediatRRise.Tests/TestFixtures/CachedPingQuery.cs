using MediatRRise.Core.Abstractions;

namespace MediatRRise.Tests.TestFixtures;
public record CachedPingQuery(string Input) : ICacheableRequest<string>
{
    public string CacheKey => $"CachedPing:{Input}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}

public class CachedPingHandler : IRequestHandler<CachedPingQuery, string>
{
    public Task<string> Handle(CachedPingQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Cached Pong: {request.Input}");
    }
}