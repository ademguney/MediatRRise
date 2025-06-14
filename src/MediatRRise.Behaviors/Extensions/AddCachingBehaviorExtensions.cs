using MediatRRise.Behaviors.Caching;
using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Behaviors.Extensions;

/// <summary>
/// Extension methods for adding caching behavior to the MediatRRise pipeline.
/// </summary>
public static class AddCachingBehaviorExtensions
{
    public static IServiceCollection AddCachingBehavior(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        return services;
    }
}