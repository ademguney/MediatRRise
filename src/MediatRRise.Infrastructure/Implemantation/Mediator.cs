using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace MediatRRise.Infrastructure.Implemantation;

/// <summary>
/// Default implementation of the IMediator interface.
/// Resolves and executes appropriate handlers and behaviors for requests and notifications.
/// </summary>
public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private static readonly ConcurrentDictionary<Type, Func<object, object, CancellationToken, Task<object>>> _handlerExecutors = new();

    /// <summary>
    /// Sends a request that returns a response, passing it through any registered pipeline behaviors.
    /// </summary>
    /// <typeparam name="TResponse">Type of the expected response.</typeparam>
    /// <param name="request">The request object implementing IRequest&lt;TResponse&gt;.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The result from the request handler.</returns>
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        var handler = serviceProvider.GetRequiredService(handlerType);

        var executor = _handlerExecutors.GetOrAdd(
            handlerType,
            CreateExecutor<TResponse>(requestType, handlerType)
        );

        var pipelineExecutor = new PipelineExecutor(serviceProvider);
        var method = typeof(PipelineExecutor)
            .GetMethod(nameof(PipelineExecutor.Execute))!
            .MakeGenericMethod(requestType, responseType);

        var task = (Task<TResponse>)method.Invoke(pipelineExecutor, new object[]
        {
            request,
            cancellationToken,
            (Func<object, CancellationToken, Task<TResponse>>)((handlerObj, token) => executor(handlerObj, request, token).ContinueWith(t => (TResponse)t.Result!))
        })!;

        return await task;
    }

    /// <summary>
    /// Sends a fire-and-forget style request that does not return a value.
    /// </summary>
    /// <param name="request">The request object implementing IRequest.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the execution.</returns>
    public Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var handler = serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("Handle")!;
        return (Task)method.Invoke(handler, new object[] { request, cancellationToken })!;
    }

    /// <summary>
    /// Publishes a notification to all corresponding handlers.
    /// All handlers are executed asynchronously in parallel.
    /// </summary>
    /// <typeparam name="TNotification">Type of the notification.</typeparam>
    /// <param name="notification">The notification instance.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous dispatch.</returns>
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>();
        var tasks = handlers.Select(h => h.Handle(notification, cancellationToken));
        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Creates a compiled delegate that wraps the handler's Handle method.
    /// This improves performance by avoiding reflection at runtime.
    /// </summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="requestType">The runtime request type.</param>
    /// <param name="handlerType">The type of the handler.</param>
    /// <returns>A delegate that invokes the handler.</returns>
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
