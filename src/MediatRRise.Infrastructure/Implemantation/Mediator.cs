using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace MediatRRise.Infrastructure.Implemantation;

/// <summary>
/// Default implementation of the IMediator interface.
/// Resolves and executes appropriate handlers for requests and notifications.
/// </summary>
public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private readonly PipelineExecutor _pipelineExecutor = new(serviceProvider);

    /// <summary>
    /// Sends a request that returns a response.
    /// Resolves the appropriate IRequestHandler via DI and executes it through the pipeline.
    /// </summary>
    /// <typeparam name="TResponse">Type of response expected from the request.</typeparam>
    /// <param name="request">Request object implementing IRequest&lt;TResponse&gt;.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The result from the request handler.</returns>
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = serviceProvider.GetRequiredService(handlerType);

        var executor = _handlerExecutors.GetOrAdd(
            handlerType,
            CreateExecutor<TResponse>(requestType, handlerType)
        );

        var result = await executor(handler, request, cancellationToken);
        return (TResponse)result!;
    }

    /// <summary>
    /// Publishes a notification to all corresponding INotificationHandler instances.
    /// All handlers are executed asynchronously in parallel.
    /// </summary>
    /// <typeparam name="TNotification">The type of the notification.</typeparam>
    /// <param name="notification">The event object.</param>
    /// <param name="cancellationToken">Cancellation support.</param>
    /// <returns>A task representing the async notification dispatch.</returns>
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>();
        var tasks = handlers.Select(h => h.Handle(notification, cancellationToken));
        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Sends a request that does not return a value.
    /// Useful for fire-and-forget or command-style operations.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the execution.</returns>
    public Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var handler = serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("Handle")!;
        return (Task)method.Invoke(handler, new object[] { request, cancellationToken })!;
    }

    // ---------- PRIVATE EXECUTOR CACHING  ----------

    private static readonly ConcurrentDictionary<Type, Func<object, object, CancellationToken, Task<object>>> _handlerExecutors = new();

    /// <summary>
    /// Builds and caches a compiled delegate for the given request/handler type combination.
    /// This enables high-performance invocation of IRequestHandler.Handle methods without dynamic or reflection at runtime.
    /// </summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="requestType">The runtime type of the request.</param>
    /// <param name="handlerType">The resolved handler type from DI.</param>
    /// <returns>A compiled delegate for executing the handler.</returns>
    private static Func<object, object, CancellationToken, Task<object>> CreateExecutor<TResponse>(Type requestType, Type handlerType)
    {
        var handlerParam = Expression.Parameter(typeof(object), "handler");
        var requestParam = Expression.Parameter(typeof(object), "request");
        var tokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        var castedHandler = Expression.Convert(handlerParam, handlerType);
        var castedRequest = Expression.Convert(requestParam, requestType);

        var method = handlerType.GetMethod("Handle")!;
        var call = Expression.Call(castedHandler, method, castedRequest, tokenParam);

        var lambda = Expression.Lambda<Func<object, object, CancellationToken, Task<TResponse>>>(
            call,
            handlerParam,
            requestParam,
            tokenParam
        ).Compile();

        return async (handler, request, token) =>
        {
            var result = await lambda(handler, request, token);
            return (object)result!;
        };
    }
}