using MediatRRise.Behaviors.Logging;
using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Behaviors.Extensions;
public static class AddLoggingBehaviorExtensions
{
    public static IServiceCollection AddLoggingBehavior(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return services;
    }
}