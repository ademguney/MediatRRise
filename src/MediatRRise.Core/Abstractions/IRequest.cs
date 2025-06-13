namespace MediatRRise.Core.Abstractions;

/// <summary>
/// Marker interface for a request with a response.
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IRequest<out TResponse> { }