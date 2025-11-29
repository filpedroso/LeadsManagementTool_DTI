namespace LeadsManagement.Domain.Entities;

using Enums;
using ValueObjects;

// Lead entity class
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

    // Constructors

    // Parameterless ctor for Entity Framework, who runs the query,
    // uses reflection to create an empty Lead with the parameterless ctor, 
    // uses private setters (above) for "hidrating" the Lead,
    // filling it appropriately with DB data, from a previous mapping object-DB
    // null! value tells the compiler that the object will be filled with non null values
    private Lead()
    {
        Contact = null!;
        Suburb = null!;
        Category = null!;
        Description = null!;
        Price = null!;
    }

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

    // Applies discount if > $500 upon acceptance
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
