using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Extensions;
using MediatRRise.Tests.TestFixtures;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentAssertions;


namespace MediatRRise.Tests.DI;

public class ServiceRegistrationTests
{
    [Fact]
    public void AddMediator_Should_Register_IMediator_And_Handlers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act: Register using extension method
        services.AddMediator(typeof(PingQueryHandler)); // marker type

        var provider = services.BuildServiceProvider();

        // Assert: IMediator resolved
        var mediator = provider.GetService<IMediator>();
        Assert.NotNull(mediator);

        // Assert: PingQueryHandler resolved via IRequestHandler
        var handler = provider.GetService<IRequestHandler<PingQuery, string>>();
        Assert.NotNull(handler);
        Assert.IsType<PingQueryHandler>(handler);
    }

    [Fact]
    public void AddMediator_should_register_handlers_via_assembly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediator(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        var provider = services.BuildServiceProvider();

        // Assert
        var mediator = provider.GetService<IMediator>();
        mediator.Should().NotBeNull();

        // örnek bir handler'a ait servis var mı?
        var handler = provider.GetServices(typeof(IRequestHandler<PingQuery, string>));
        handler.Should().NotBeEmpty();
    }
}