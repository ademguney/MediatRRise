using MediatRRise.Behaviors.Retry;
using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Behaviors.Extensions;

/// <summary>
/// Adds logging behavior to the mediator pipeline.
/// Logs before and after the request is handled.
/// </summary>
public static class AddRetryBehaviorExtensions
{
    public static IServiceCollection AddRetryBehavior(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));
        return services;
    }
}