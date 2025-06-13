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
    /// Publishes a notification with type safety.
    /// </summary>
    /// <typeparam name="TNotification">Notification type.</typeparam>
    /// <param name="notification">Notification instance.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}