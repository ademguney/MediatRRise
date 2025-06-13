namespace MediatRRise.Core.Abstractions;

/// <summary>
/// Handles an <see cref="INotification"/> event without returning a result.
/// Multiple handlers can listen to the same event.
/// </summary>
/// <typeparam name="TNotification">The notification type.</typeparam>
public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    /// <summary>
    /// Handles the notification asynchronously.
    /// </summary>
    /// <param name="notification">The notification instance.</param>
    /// <param name="cancellationToken">Cancellation support.</param>
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}