using MediatRRise.Behaviors.Logging;
using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Behaviors.Extensions;

/// <summary>
/// Adds logging behavior to the MediatRRise pipeline.
/// Logs before and after a request is handled.
/// </summary>
public static class AddLoggingBehaviorExtensions
{
    /// <summary>
    /// Registers <see cref="LoggingBehavior{TRequest, TResponse}"/> into the MediatRRise pipeline.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddLoggingBehavior(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return services;
    }
}