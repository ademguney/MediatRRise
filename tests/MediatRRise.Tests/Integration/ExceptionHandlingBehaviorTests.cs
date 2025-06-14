using MediatRRise.Behaviors.ExceptionHandling;
using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Extensions;
using MediatRRise.Tests.TestFixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediatRRise.Tests.Integration;

public class ExceptionHandlingBehaviorTests
{
    [Fact]
    public async Task ExceptionHandlingBehavior_Should_Log_And_Rethrow_Exception()
    {
        // Arrange
        var services = new ServiceCollection();

        var loggerMock = new Mock<ILogger<ExceptionHandlingBehavior<IRequest<string>, string>>>();

        services.AddSingleton(loggerMock.Object);

        services.AddLogging();

        services.AddMediator(typeof(FaultyRequestHandler));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            mediator.Send(new FaultyRequest("Trigger")));

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("[Exception]")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}