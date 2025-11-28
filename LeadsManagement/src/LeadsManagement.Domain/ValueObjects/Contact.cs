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
