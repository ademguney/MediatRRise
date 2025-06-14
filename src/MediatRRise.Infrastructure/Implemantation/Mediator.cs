using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Infrastructure.Implemantation;

/// <summary>
/// Default implementation of the IMediator interface.
/// Resolves and executes appropriate handlers for requests and notification.
/// </summary>
public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private readonly PipelineExecutor _pipelineExecutor = new(serviceProvider);

    /// <summary>
    /// Publishes a notification event to all registered <see cref="INotificationHandler{TNotification}"/> handlers.
    /// </summary>
    /// <param name="notification">The notification/event instance to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that completes when all handlers have finished processing.</returns>
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
       where TNotification : INotification
    {
        var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>();

        var tasks = handlers.Select(h => h.Handle(notification, cancellationToken));

        return Task.WhenAll(tasks);
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
        dynamic handler = serviceProvider.GetRequiredService(handlerType);

        Task<TResponse> HandlerFunc(IRequest<TResponse> _, CancellationToken token) =>
            handler.Handle((dynamic)request, token);

        return _pipelineExecutor.Execute(request, cancellationToken, HandlerFunc);
    }

    /// <summary>
    /// Sends a request to the appropriate <see cref="IRequestHandler{TRequest}"/>
    /// that does not return a value.
    /// </summary>
    /// <param name="request">The request instance to process.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that completes when the handler has finished processing.</returns>
    public Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        var handler = serviceProvider.GetRequiredService<IRequestHandler<IRequest>>();
        return handler.Handle(request, cancellationToken);
    }
}