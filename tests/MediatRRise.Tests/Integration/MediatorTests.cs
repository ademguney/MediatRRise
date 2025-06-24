using MediatRRise.Core.Abstractions;
using MediatRRise.Infrastructure.Implemantation;
using MediatRRise.Tests.TestFixtures;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRRise.Tests.Integration;

public class MediatorTests
{
    private readonly IMediator _mediator;

    public MediatorTests()
    {
        var services = new ServiceCollection();

        // Register PingQueryHandler
        services.AddTransient<IRequestHandler<PingQuery, string>, PingQueryHandler>();

        // Register MediatRRise
        services.AddSingleton<IMediator, Mediator>();

        var provider = services.BuildServiceProvider();
        _mediator = provider.GetRequiredService<IMediator>();
    }


    [Fact]
    public async Task Send_Should_Invoke_Handler_And_Return_Result()
    {
        // Arrange
        var query = new PingQuery("Hello, MediatR Rise!");

        // Act
        var result = await _mediator.Send(query);

        // Assert
        Assert.Equal("Pong: Hello, MediatR Rise!", result);
    }

    [Fact]
    public async Task Send_SameQueryTwice_UsesCachedHandler()
    {
        var query1 = new PingQuery("1");
        var query2 = new PingQuery("2");

        var result1 = await _mediator.Send(query1);
        var result2 = await _mediator.Send(query2);

        Assert.Equal("Pong: 1", result1);
        Assert.Equal("Pong: 2", result2);
    }
}