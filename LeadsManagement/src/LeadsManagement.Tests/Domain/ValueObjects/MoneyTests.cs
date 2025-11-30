using Xunit;
using FluentAssertions;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Money_WhenCreatedWithPositiveAmount_ShouldSucceed()
    {
        // Act
        var money = new Money(100);

        // Assert
        money.Amount.Should().Be(100);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Money_WhenCreatedWithCustomCurrency_ShouldSetCurrency()
    {
        // Act
        var money = new Money(100, "EUR");

        // Assert
        money.Amount.Should().Be(100);
        money.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Money_WhenCreatedWithNegativeAmount_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new Money(-100);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Amount cannot be negative*")
            .WithParameterName("amount");
    }

    [Fact]
    public void ApplyDiscount_WhenValidDiscount_ShouldReturnDiscountedMoney()
    {
        // Arrange
        var money = new Money(1000);

        // Act
        var discounted = money.ApplyDiscount(0.10m);

        // Assert
        discounted.Amount.Should().Be(900);
        discounted.Currency.Should().Be("USD");
    }

    [Fact]
    public void ApplyDiscount_WhenDiscountIsNegative_ShouldThrowArgumentException()
    {
        // Arrange
        var money = new Money(1000);

        // Act
        Action act = () => money.ApplyDiscount(-0.1m);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Discount percentage must be between 0 and 1*")
            .WithParameterName("discountPercentage");
    }

    [Fact]
    public void ApplyDiscount_WhenDiscountIsGreaterThan1_ShouldThrowArgumentException()
    {
        // Arrange
        var money = new Money(1000);

        // Act
        Action act = () => money.ApplyDiscount(1.5m);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Discount percentage must be between 0 and 1*")
            .WithParameterName("discountPercentage");
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var money = new Money(1234.56m);

        // Act
        var result = money.ToString();

        // Assert
        result.Should().Be("1234.56 USD");
    }

    [Fact]
    public void ImplicitConversion_FromDecimal_ShouldCreateMoney()
    {
        // Act
        Money money = 500m;

        // Assert
        money.Amount.Should().Be(500);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void ImplicitConversion_ToDecimal_ShouldReturnAmount()
    {
        // Arrange
        var money = new Money(750);

        // Act
        decimal amount = money;

        // Assert
        amount.Should().Be(750);
    }
}
