using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Infrastructure.Implemantation;

/// <summary>
/// Default implementation of the IMediator interface.
/// Resolves and executes appropriate handlers for requests and notification.
/// </summary>
public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    /// <summary>
    /// Publishes a notification event to all registered <see cref="INotificationHandler{TNotification}"/> handlers.
    /// </summary>
    /// <param name="notification">The notification/event instance to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that completes when all handlers have finished processing.</returns>
    public async Task Publish(INotification notification, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handlers = serviceProvider.GetServices(handlerType);

        if (handlers == null || !handlers.Any())
            throw new InvalidOperationException($"No handlers found for notification type {notification.GetType().Name}");

        foreach (var handler in handlers)
        {
            await ((dynamic)handler).Handle((dynamic)notification, cancellationToken);
        }
    }

    /// <summary>
    /// Sends a request to the appropriate <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// and returns a response.
    /// </summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="request">The request instance to process.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The response returned by the handler.</returns>
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        dynamic? handler = serviceProvider.GetService(handlerType);

        if (handler is null)
            throw new InvalidOperationException($"No handler found for request type {request.GetType().Name}");

        return ((dynamic)handler).Handle((dynamic)request, cancellationToken);
    }

    /// <summary>
    /// Sends a request to the appropriate <see cref="IRequestHandler{TRequest}"/>
    /// that does not return a value.
    /// </summary>
    /// <param name="request">The request instance to process.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that completes when the handler has finished processing.</returns>
    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
        dynamic? handler = serviceProvider.GetService(handlerType);

        if (handler is null)
            throw new InvalidOperationException($"No handler found for request type {request.GetType().Name}");

        await handler.Handle((dynamic)request, cancellationToken);
    }
}