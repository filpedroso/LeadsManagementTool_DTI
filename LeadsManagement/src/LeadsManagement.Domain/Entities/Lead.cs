namespace LeadsManagement.Domain.Entities;

using Enums;
using ValueObjects;

// Entidade Lead -> 
public class Lead
{
    public int Id { get; private set; }

    public Contact Contact { get; private set; }

    public DateTime DateCreated { get; private set; }

    public string Suburb { get; private set; }

    public string Category { get; private set; }

    public string Description { get; private set; }

    public Money Price { get; private set; }

    public LeadStatus Status { get; private set; }

    // Construtores
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

    // Aplica desconto se preço > $500
    public void Accept()
    {
        if (Status != LeadStatus.Invited)
            throw new InvalidOperationException($"Cannot accept a lead with status {Status}");

        if (Price.Amount > 500)
        {
            Price = Price.ApplyDiscount(0.10m);
        }
        Status = LeadStatus.Accepted;
    }

    public void Decline()
    {
        if (Status != LeadStatus.Invited)
            throw new InvalidOperationException($"Cannot decline a lead with status {Status}");

        Status = LeadStatus.Declined;
    }
}
