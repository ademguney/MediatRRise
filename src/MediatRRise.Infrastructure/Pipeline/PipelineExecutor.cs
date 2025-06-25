using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Infrastructure.Pipeline;

/// <summary>
/// Executes a request through a pipeline of behaviors.
/// </summary>
internal class PipelineExecutor(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Executes the given request through all registered pipeline behaviors,
    /// ultimately calling the provided request handler function.
    /// </summary>
    /// <typeparam name="TRequest">The type of request.</typeparam>
    /// <typeparam name="TResponse">The type of response.</typeparam>
    /// <param name="request">The request instance.</param>
    /// <param name="cancellationToken">Cancellation support.</param>
    /// <param name="handlerFunc">The final handler function to be called.</param>
    /// <returns>The result of the handler execution.</returns>
    public Task<TResponse> Execute<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken,
        Func<TRequest, CancellationToken, Task<TResponse>> handlerFunc)
    {
        var behaviors = serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse() // First registered runs outermost
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