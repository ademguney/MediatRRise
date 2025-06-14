using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Infrastructure.Pipeline;

/// <summary>
/// Executes a request through a pipeline of behaviors.
/// </summary>
/// <param name="serviceProvider"></param>
internal class PipelineExecutor(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Executes a request through the pipeline of behaviors and returns a response.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="handlerFunc"></param>
    /// <returns></returns>
    public Task<TResponse> Execute<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken,
        Func<TRequest, CancellationToken, Task<TResponse>> handlerFunc)
        where TRequest : IRequest<TResponse>
    {
        var behaviors = serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse() // Reverse so first registered behavior runs first
            .ToList();

        RequestHandlerDelegate<TResponse> next = () => handlerFunc(request, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var current = next;
            next = () => behavior.Handle(request, current, cancellationToken);
        }

        return next();
    }
}