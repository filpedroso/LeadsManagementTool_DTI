namespace LeadsManagement.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um valor monetário
/// Encapsula lógica de negócio relacionada a preços
/// </summary>
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

    /// <summary>
    /// Aplica desconto ao valor
    /// </summary>
    /// <param name="discountPercentage">Percentual de desconto (0-1)</param>
    /// <returns>Novo Money com desconto aplicado</returns>
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
