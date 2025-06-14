using MediatRRise.Behaviors.Extensions;
using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Extensions;
using MediatRRise.Tests.TestFixtures;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace MediatRRise.Tests.Integration;

public class LoggingBehaviorTests
{
    [Fact]
    public async Task LoggingBehavior_Should_Log_Request_Lifecycle()
    {
        // Arrange
        var services = new ServiceCollection();
        var output = new StringBuilder();
        var writer = new StringWriter(output);
        Console.SetOut(writer); // redirect console output

        services.AddMediator(typeof(PingHandler));
        services.AddLoggingBehavior();

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var result = await mediator.Send(new PingQuery("Hello"));

        // Assert
        var logs = output.ToString();
        Assert.Contains("[Logging] Handling request: PingQuery", logs);
        Assert.Contains("[Logging] Handled request: PingQuery", logs);
        Assert.Equal("Pong: Hello", result);
    }
}