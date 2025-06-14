using MediatRRise.Behaviors.Retry;
using MediatRRise.Core.Abstractions;
using MediatRRise.Tests.TestFixtures;
using Moq;

namespace MediatRRise.Tests.Integration;

public class RetryBehaviorTests
{
    [Fact]
    public async Task Should_Throw_Exception_After_Max_Retries()
    {
        // Arrange
        var retryCount = 3;
        var behavior = new RetryBehavior<FakeRetryRequest, string>(retryCount, delayMilliseconds: 10);

        var mockNext = new Mock<RequestHandlerDelegate<string>>();
        mockNext.Setup(n => n()).ThrowsAsync(new TimeoutException("Timeout!"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TimeoutException>(() =>
            behavior.Handle(new FakeRetryRequest(), mockNext.Object, CancellationToken.None));

        mockNext.Verify(n => n(), Times.Exactly(retryCount));
        Assert.Equal("Timeout!", exception.Message);
    }

    [Fact]
    public async Task Should_Succeed_If_Handler_Succeeds_On_Second_Attempt()
    {
        // Arrange
        var retryCount = 3;
        var callCount = 0;

        var behavior = new RetryBehavior<FakeRetryRequest, string>(retryCount, delayMilliseconds: 10);

        var mockNext = new Mock<RequestHandlerDelegate<string>>();
        mockNext.Setup(n => n()).Returns(() =>
        {
            callCount++;
            if (callCount == 1)
                throw new TimeoutException();
            return Task.FromResult("OK");
        });

        // Act
        var result = await behavior.Handle(new FakeRetryRequest(), mockNext.Object, CancellationToken.None);

        // Assert
        Assert.Equal("OK", result);
        Assert.Equal(2, callCount);
    }
}