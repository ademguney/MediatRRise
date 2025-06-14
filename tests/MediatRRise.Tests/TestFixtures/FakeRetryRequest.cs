using MediatRRise.Core.Abstractions;

namespace MediatRRise.Tests.TestFixtures;
public record FakeRetryRequest : IRequest<string>;