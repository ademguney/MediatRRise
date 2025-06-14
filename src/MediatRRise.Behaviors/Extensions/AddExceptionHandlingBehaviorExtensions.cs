using MediatRRise.Behaviors.ExceptionHandling;
using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Behaviors.Extensions;

/// <summary>
/// Adds exception handling behavior to the MediatRRise pipeline.
/// Catches and logs unhandled exceptions during request hangling.
/// </summary>
public static class AddExceptionHandlingBehaviorExtensions
{
    /// <summary>
    /// Registers <see cref="ExceptionHandlingBehavior{TRequest, TResponse}"/> into the MediatRRise pipeline.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddExceptionHandlingBehavior(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
        return services;
    }
}