using FluentValidation;
using MediatRRise.Behaviors.Validation;
using MediatRRise.Core.Abstractions;
using MediatRRise.Tests.TestFixtures;
using Moq;

namespace MediatRRise.Tests.Integration;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task ValidationBehavior_Should_Throw_ValidationException_When_Request_Is_Invalid()
    {
        // Arrange
        var validators = new List<IValidator<FakeRequest>> { new FakeValidator() };

        var behavior = new ValidationBehavior<FakeRequest, string>(validators);

        var request = new FakeRequest(""); // invalid: Email is emptpy

        var mockNext = new Mock<RequestHandlerDelegate<string>>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(request, mockNext.Object, CancellationToken.None)
        );

        Assert.Contains("Email is required", exception.Message);
    }

    [Fact]
    public async Task ValidationBehavior_Should_Invoke_Next_When_Request_Is_Valid()
    {
        // Arrange
        var validators = new List<IValidator<FakeRequest>> { new FakeValidator() };

        var behavior = new ValidationBehavior<FakeRequest, string>(validators);

        var request = new FakeRequest("test@example.com"); // valid

        var mockNext = new Mock<RequestHandlerDelegate<string>>();
        mockNext.Setup(n => n()).ReturnsAsync("OK");

        // Act
        var result = await behavior.Handle(request, mockNext.Object, CancellationToken.None);

        // Assert
        Assert.Equal("OK", result);
    }
}