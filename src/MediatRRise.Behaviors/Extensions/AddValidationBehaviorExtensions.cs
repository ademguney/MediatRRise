using MediatRRise.Behaviors.Validation;
using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Behaviors.Extensions;

/// <summary>
/// Adds validation behavior to the MediatRRise pipeline.
/// Executes validation on incoming requests before they are handled.
/// </summary>
public static class AddValidationBehaviorExtensions
{
    /// <summary>
    /// Registers <see cref="ValidationBehavior{TRequest, TResponse}"/> into the MediatRRise pipeline.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddValidationBehavior(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}