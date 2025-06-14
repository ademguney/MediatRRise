using MediatRRise.Behaviors.Performance;
using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Behaviors.Extensions;

/// <summary>
/// Registers <see cref="PerformanceBehavior{TRequest, TResponse}"/> into the pipeline.
/// </summary>
public static class AddPerformanceBehaviorExtensions
{
    /// <summary>
    /// Registers the performance behavior into the MediatRRise pipeline.
    /// </summary>
    public static IServiceCollection AddPerformanceBehavior(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        return services;
    }
}