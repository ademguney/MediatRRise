using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Infrastructure.Pipeline;

internal class PipelineExecutor(IServiceProvider serviceProvider)
{
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