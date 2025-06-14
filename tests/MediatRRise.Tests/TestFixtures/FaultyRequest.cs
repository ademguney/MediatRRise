using MediatRRise.Core.Abstractions;

namespace MediatRRise.Tests.TestFixtures;
public record FaultyRequest(string Input) : IRequest<string>;

public class FaultyRequestHandler : IRequestHandler<FaultyRequest, string>
{
    public Task<string> Handle(FaultyRequest request, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("Intentional crash!");
    }
}