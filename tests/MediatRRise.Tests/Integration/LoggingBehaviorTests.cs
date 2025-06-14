using MediatRRise.Behaviors.Logging;
using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Implemantation;
using MediatRRise.Tests.TestFixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediatRRise.Tests.Integration;

public class LoggingBehaviorTests
{
    [Fact]
    public async Task LoggingBehavior_Should_Log_Request_Lifecycle()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<LoggingBehavior<PingQuery, string>>>();

        var behavior = new LoggingBehavior<PingQuery, string>(loggerMock.Object);

        var request = new PingQuery("Hello");
        var response = "Pong: Hello";

        RequestHandlerDelegate<string> next = () => Task.FromResult(response);

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("[Logging] Handling request: PingQuery")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("[Logging] Handled request: PingQuery")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        Assert.Equal("Pong: Hello", result);
    }


}