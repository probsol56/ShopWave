using FluentAssertions;
using ShopWave.Application.Features.Auth.Commands.Register;

namespace ShopWave.UnitTests.Features.Auth;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldPassValidation()
    {
        var command = new RegisterCommand(
            "Acme Corp", "acme-corp",
            "owner@acme.com", "SecurePass1!",
            "John", "Doe");

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task InvalidSlug_WithUppercase_ShouldFailValidation()
    {
        var command = new RegisterCommand(
            "Acme Corp", "Acme-Corp",
            "owner@acme.com", "SecurePass1!",
            "John", "Doe");

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TenantSlug");
    }

    [Fact]
    public async Task InvalidEmail_ShouldFailValidation()
    {
        var command = new RegisterCommand(
            "Acme Corp", "acme-corp",
            "not-an-email", "SecurePass1!",
            "John", "Doe");

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task ShortPassword_ShouldFailValidation()
    {
        var command = new RegisterCommand(
            "Acme Corp", "acme-corp",
            "owner@acme.com", "123",
            "John", "Doe");

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}
