namespace MediatRRise.Core.Abstractions;

/// <summary>
/// Handles a request with no response.
/// </summary>
public interface IRequestHandler<in TRequest> where TRequest : IRequest
{
    Task Handle(TRequest request, CancellationToken cancellationToken);
}