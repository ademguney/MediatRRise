namespace MediatRRise.Core.Abstractions;

/// <summary>
/// Central interface to send requests and publish notifications.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a request to a single matching handler and returns a response.
    /// </summary>
    /// <typeparam name="TResponse">The type of response.</typeparam>
    /// <param name="request">The request instance.</param>
    /// <param name="cancellationToken">Cancellation support.</param>
    /// <returns>The response from the handler.</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request that does not return a response.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <param name="cancellationToken">Cancellation support.</param>
    Task Send(IRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a notification to all matching handlers.
    /// </summary>
    /// <param name="notification">The notification instance.</param>
    /// <param name="cancellationToken">Cancellation support.</param>
    Task Publish(INotification notification, CancellationToken cancellationToken = default);
}