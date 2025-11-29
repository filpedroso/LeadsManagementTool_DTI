namespace LeadsManagement.Domain.ValueObjects;

// Value Object for Money and its business logic
// "init" is a init-only setter method
public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        Amount = amount;
        Currency = currency;
    }

    // Method for applying discount to itself 
    // and returning the new Money object with the discount applied
    public Money ApplyDiscount(decimal discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 1)
            throw new ArgumentException("Discount percentage must be between 0 and 1", nameof(discountPercentage));

        var discountedAmount = Amount - (Amount * discountPercentage);
        return new Money(discountedAmount, Currency);
    }

    public static implicit operator decimal(Money money) => money.Amount;
    public static implicit operator Money(decimal amount) => new(amount);

    public override string ToString() => $"{Amount:F2} {Currency}";
}
