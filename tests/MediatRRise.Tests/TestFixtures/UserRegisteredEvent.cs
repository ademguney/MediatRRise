using MediatRRise.Core.Abstractions;

namespace MediatRRise.Tests.TestFixtures;

public record UserRegisteredEvent(string Email) : INotification;

public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    public static bool WasCalled { get; private set; } = false;

    public Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        WasCalled = true;
        return Task.CompletedTask;
    }

    public static void Reset() => WasCalled = false;
}