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
