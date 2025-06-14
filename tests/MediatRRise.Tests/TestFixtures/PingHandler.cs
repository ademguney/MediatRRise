using MediatRRise.Core.Abstractions;

namespace MediatRRise.Tests.TestFixtures;

public class PingHandler : IRequestHandler<PingQuery, string>
{
    public Task<string> Handle(PingQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"PONG: {request.Message}");
    }
}