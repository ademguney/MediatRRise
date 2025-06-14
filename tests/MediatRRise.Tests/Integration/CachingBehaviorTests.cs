using MediatRRise.Behaviors.Caching;
using MediatRRise.Behaviors.Extensions;
using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Extensions;
using MediatRRise.Tests.TestFixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediatRRise.Tests.Integration;

public class CachingBehaviorTests
{
    [Fact]
    public async Task CachingBehavior_Should_Store_And_Return_Cached_Response()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        var cacheMock = new Mock<ICacheService>();
        var loggerMock = new Mock<ILogger<CachingBehavior<CachedPingQuery, string>>>();

        // Setup: ilk çağrıda cache boş, ikincisinde veri döner
        cacheMock.SetupSequence(x => x.GetAsync<string>("CachedPing:Hello", It.IsAny<CancellationToken>()))
                 .ReturnsAsync((string?)null)                      // 1. çağrıda cache'te yok
                 .ReturnsAsync("Cached Pong: Hello");              // 2. çağrıda cache'te var

        services.AddSingleton(cacheMock.Object);
        services.AddSingleton(loggerMock.Object);
        services.AddMediator(typeof(CachedPingHandler));
        services.AddCachingBehavior();

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var query = new CachedPingQuery("Hello");

        // Act – 1. çağrı: veriyi üret ve cache'e yaz
        var response1 = await mediator.Send(query);

        // Act – 2. çağrı: cache'den oku
        var response2 = await mediator.Send(query);

        // Assert
        Assert.Equal("Cached Pong: Hello", response1);
        Assert.Equal("Cached Pong: Hello", response2);

        // SetAsync sadece 1 kez çağrılmış olmalı
        cacheMock.Verify(x =>
            x.SetAsync("CachedPing:Hello", "Cached Pong: Hello", TimeSpan.FromMinutes(5), It.IsAny<CancellationToken>()),
            Times.Once);

        // GetAsync ise 2 kez çağrılmalı
        cacheMock.Verify(x =>
            x.GetAsync<string>("CachedPing:Hello", It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }
}