using FluentAssertions;
using ShopWave.Application.Features.Shops.Commands.CreateShop;

namespace ShopWave.UnitTests.Features.Shops;

public class CreateShopCommandValidatorTests
{
    private readonly CreateShopCommandValidator _validator = new();

    [Fact]
    public async Task ValidCommand_ShouldPassValidation()
    {
        var command = new CreateShopCommand("My Shop", "123 Main St", "+1234567890", "USD");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task EmptyName_ShouldFailValidation()
    {
        var command = new CreateShopCommand("", null, null, "USD");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task InvalidCurrency_WrongLength_ShouldFailValidation()
    {
        var command = new CreateShopCommand("My Shop", null, null, "US");
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Currency");
    }
}
