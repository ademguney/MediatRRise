using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Implemantation;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering MediatR Rise services into the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers IMediator and all handlers found in the given assemblies.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="markerTypes">Any type within the target assemblies (used to scan handlers).</param>
    /// <returns>The same IServiceCollection instance.</returns>
    public static IServiceCollection AddMediator(this IServiceCollection services, params Type[] markerTypes)
    {
        services.AddSingleton<IMediator, Mediator>();

        var assemblies = markerTypes.Select(t => t.Assembly).Distinct().ToArray();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();

            var requestHandlers = types
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && (
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)
                    ))
                    .Select(i => new { Service = i, Implementation = t }));

            var notificationHandlers = types
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                    .Select(i => new { Service = i, Implementation = t }));

            foreach (var handler in requestHandlers.Concat(notificationHandlers))
            {
                services.AddTransient(handler.Service, handler.Implementation);
            }
        }

        return services;
    }
}