using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Implemantation;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering MediatRRise services into the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers IMediator and all IRequestHandler/INotificationHandler implementations
    /// from the assemblies of the specified marker types.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="markerTypes">Marker types used to identify target assemblies.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddMediator(this IServiceCollection services, params Type[] markerTypes)
    {
        var assemblies = markerTypes.Select(t => t.Assembly).Distinct().ToArray();
        return services.AddMediator(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
    }

    /// <summary>
    /// Registers IMediator and all IRequestHandler/INotificationHandler implementations
    /// using the specified configuration options.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="optionsBuilder">An action to configure assembly scanning options.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddMediator(this IServiceCollection services, Action<MediatRRiseOptions> optionsBuilder)
    {
        var options = new MediatRRiseOptions();
        optionsBuilder.Invoke(options);

        services.AddScoped<IMediator, Mediator>();

        foreach (var assembly in options.Assemblies.Distinct())
        {
            var types = assembly.GetTypes();

            var requestHandlers = types
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                         i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)))
                    .Select(i => new { Service = i, Implementation = t }));

            var notificationHandlers = types
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                    .Select(i => new { Service = i, Implementation = t }));

            foreach (var handler in requestHandlers.Concat(notificationHandlers))
            {
                services.AddTransient(handler.Service, handler.Implementation);
            }
        }

        return services;
    }
}
