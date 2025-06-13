using MediatRRise.Core.Abstractions;

namespace MediatRRise.Tests.TestFixtures;
public record PingQuery(string Message) : IRequest<string>;

public class PingQueryHandler : IRequestHandler<PingQuery, string>
{
    public Task<string> Handle(PingQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Pong: {request.Message}");
    }
}