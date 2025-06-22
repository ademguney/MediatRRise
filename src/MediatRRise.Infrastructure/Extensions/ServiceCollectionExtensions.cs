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
    /// Original AddMediator with marker type(s).
    /// </summary>
    public static IServiceCollection AddMediator(this IServiceCollection services, params Type[] markerTypes)
    {
        var assemblies = markerTypes.Select(t => t.Assembly).Distinct().ToArray();

        return services.AddMediator(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
    }

    /// <summary>
    /// New overload to support MediatR-style registration from assemblies.
    /// </summary>
    public static IServiceCollection AddMediator(this IServiceCollection services, Action<MediatRRiseOptions> optionsBuilder)
    {
        var options = new MediatRRiseOptions();
        optionsBuilder.Invoke(options);

        services.AddSingleton<IMediator, Mediator>();

        foreach (var assembly in options.Assemblies.Distinct())
        {
            var types = assembly.GetTypes();

            var requestHandlers = types
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && (
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
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