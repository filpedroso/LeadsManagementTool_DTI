// ===================================================================
// FILE: src/LeadsManagement.Domain/Enums/LeadStatus.cs
// ===================================================================

namespace LeadsManagement.Domain.Enums;

/// <summary>
/// Enum que representa os possíveis status de um Lead
/// </summary>
public enum LeadStatus
{
    /// <summary>
    /// Lead convidado (novo)
    /// </summary>
    Invited = 0,

    /// <summary>
    /// Lead aceito
    /// </summary>
    Accepted = 1,

    /// <summary>
    /// Lead recusado
    /// </summary>
    Declined = 2
}

// ===================================================================
// FILE: src/LeadsManagement.Domain/ValueObjects/Money.cs
// ===================================================================

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

// ===================================================================
// FILE: src/LeadsManagement.Domain/ValueObjects/Contact.cs
// ===================================================================

namespace LeadsManagement.Domain.ValueObjects;

/// <summary>
/// Value Object que representa as informações de contato
/// </summary>
public record Contact
{
    public string FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }

    public Contact(string firstName, string? lastName = null, string? phoneNumber = null, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));

        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public string FullName => string.IsNullOrEmpty(LastName) ? FirstName : $"{FirstName} {LastName}";
}

// ===================================================================
// FILE: src/LeadsManagement.Domain/Entities/Lead.cs
// ===================================================================

namespace LeadsManagement.Domain.Entities;

using Enums;
using ValueObjects;
using Events;

/// <summary>
/// Entidade agregada Lead - Representa um lead de venda
/// Contém toda a lógica de negócio relacionada a leads
/// </summary>
public class Lead
{
    // Properties
    public int Id { get; private set; }

    public Contact Contact { get; private set; }

    public DateTime DateCreated { get; private set; }

    public string Suburb { get; private set; }

    public string Category { get; private set; }

    public string Description { get; private set; }

    public Money Price { get; private set; }

    public LeadStatus Status { get; private set; }

    // Domain Events
    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Constructors
    private Lead() { }

    public Lead(
        Contact contact,
        string suburb,
        string category,
        string description,
        decimal price)
    {
        Contact = contact ?? throw new ArgumentNullException(nameof(contact));
        Suburb = suburb ?? throw new ArgumentException("Suburb is required", nameof(suburb));
        Category = category ?? throw new ArgumentException("Category is required", nameof(category));
        Description = description ?? throw new ArgumentException("Description is required", nameof(description));
        Price = new Money(price);
        Status = LeadStatus.Invited;
        DateCreated = DateTime.UtcNow;
    }

    // Business Methods
    /// <summary>
    /// Aceita o lead e aplica desconto se preço > $500
    /// </summary>
    public void Accept()
    {
        if (Status != LeadStatus.Invited)
            throw new InvalidOperationException($"Cannot accept a lead with status {Status}");

        // Lógica de negócio: aplicar 10% de desconto se preço > $500
        if (Price.Amount > 500)
        {
            Price = Price.ApplyDiscount(0.10m); // 10% desconto
        }

        Status = LeadStatus.Accepted;

        // Adicionar evento de domínio
        _domainEvents.Add(new LeadAcceptedEvent(
            leadId: Id,
            finalPrice: Price.Amount,
            discountApplied: Price.Amount < new Money(Price.Amount / 0.9m).Amount,
            occurredAt: DateTime.UtcNow));
    }

    /// <summary>
    /// Recusa o lead
    /// </summary>
    public void Decline()
    {
        if (Status != LeadStatus.Invited)
            throw new InvalidOperationException($"Cannot decline a lead with status {Status}");

        Status = LeadStatus.Declined;

        _domainEvents.Add(new LeadDeclinedEvent(
            leadId: Id,
            occurredAt: DateTime.UtcNow));
    }

    /// <summary>
    /// Limpa os eventos de domínio após processá-los
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}

// ===================================================================
// FILE: src/LeadsManagement.Domain/Events/DomainEvent.cs
// ===================================================================

namespace LeadsManagement.Domain.Events;

/// <summary>
/// Classe base para eventos de domínio
/// </summary>
public abstract record DomainEvent
{
    public DateTime OccurredAt { get; init; }
    public Guid Id { get; init; } = Guid.NewGuid();
}

// ===================================================================
// FILE: src/LeadsManagement.Domain/Events/LeadAcceptedEvent.cs
// ===================================================================

namespace LeadsManagement.Domain.Events;

/// <summary>
/// Evento disparado quando um lead é aceito
/// </summary>
public record LeadAcceptedEvent : DomainEvent
{
    public int LeadId { get; init; }
    public decimal FinalPrice { get; init; }
    public bool DiscountApplied { get; init; }

    public LeadAcceptedEvent(int leadId, decimal finalPrice, bool discountApplied, DateTime occurredAt)
    {
        LeadId = leadId;
        FinalPrice = finalPrice;
        DiscountApplied = discountApplied;
        OccurredAt = occurredAt;
    }
}

// ===================================================================
// FILE: src/LeadsManagement.Domain/Events/LeadDeclinedEvent.cs
// ===================================================================

namespace LeadsManagement.Domain.Events;

/// <summary>
/// Evento disparado quando um lead é recusado
/// </summary>
public record LeadDeclinedEvent : DomainEvent
{
    public int LeadId { get; init; }

    public LeadDeclinedEvent(int leadId, DateTime occurredAt)
    {
        LeadId = leadId;
        OccurredAt = occurredAt;
    }
}
