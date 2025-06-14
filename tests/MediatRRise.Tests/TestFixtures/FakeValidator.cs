using FluentValidation;
using MediatRRise.Core.Abstractions;

namespace MediatRRise.Tests.TestFixtures;

public record FakeRequest(string Email) : IRequest<string>;

public class FakeValidator : AbstractValidator<FakeRequest>
{
    public FakeValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
    }
}