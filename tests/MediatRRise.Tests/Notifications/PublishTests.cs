using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Implemantation;
using MediatRRise.Tests.TestFixtures;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Tests.Notifications;
public class PublishTests
{
    private readonly IMediator _mediator;

    public PublishTests()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IMediator, Mediator>();
        services.AddTransient<INotificationHandler<UserRegisteredEvent>, UserRegisteredEmailHandler>();
        services.AddTransient<INotificationHandler<UserRegisteredEvent>, UserRegisteredLogHandler>();

        var provider = services.BuildServiceProvider();
        _mediator = provider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task Publish_Should_Invoke_All_NotificationHandlers()
    {
        // Reset static flags
        UserRegisteredEmailHandler.Reset();
        UserRegisteredLogHandler.Reset();

        // Act
        await _mediator.Publish(new UserRegisteredEvent("test@example.com"));

        // Assert
        Assert.True(UserRegisteredEmailHandler.WasCalled);
        Assert.True(UserRegisteredLogHandler.WasCalled);
    }
}