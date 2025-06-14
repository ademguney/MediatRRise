using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Behaviors.Extensions;

/// <summary>
/// Registers all common MediatRRise pipeline behaviors:
/// Logging, Exception Handling, Validation, and Retry.
/// </summary>
public static class AddMediatRRisePipelineExtensions
{
    /// <summary>
    /// Adds the MediatRRise pipeline behaviors to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMediatRRisePipeline(this IServiceCollection services)
    {
        services
            .AddLoggingBehavior()
            .AddExceptionHandlingBehavior()
            .AddValidationBehavior()
            .AddRetryBehavior()
            .AddPerformanceBehavior()
            .AddCachingBehavior();

        return services;
    }
}