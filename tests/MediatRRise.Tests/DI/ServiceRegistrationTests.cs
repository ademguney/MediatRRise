using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Extensions;
using MediatRRise.Tests.TestFixtures;
using Microsoft.Extensions.DependencyInjection;

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
}