using MediatRRise.Behaviors.Performance;
using MediatRRise.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediatRRise.Tests.Unit.Behaviors;
public class PerformanceBehaviorTests
{
    [Fact]
    public async Task Should_Log_Warning_When_Execution_Exceeds_Threshold()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<PerformanceBehavior<FakeRequest, string>>>();

        var behavior = new PerformanceBehavior<FakeRequest, string>(
            loggerMock.Object,
            thresholdMs: 100 
        );

        var request = new FakeRequest();      
        Task<string> NextDelegate() => Task.Delay(200).ContinueWith(_ => "OK");

        // Act
        await behavior.Handle(request, NextDelegate, CancellationToken.None);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Performance")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Log_Information_When_Execution_Is_Fast()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<PerformanceBehavior<FakeRequest, string>>>();

        var behavior = new PerformanceBehavior<FakeRequest, string>(
            loggerMock.Object,
            thresholdMs: 500 
        );

        var request = new FakeRequest();

        Task<string> NextDelegate() => Task.FromResult("OK");

        // Act
        await behavior.Handle(request, NextDelegate, CancellationToken.None);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Performance")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    public class FakeRequest : IRequest<string> { }
}
