using Xunit;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Domain;

public class LeadTests
{
    [Fact]
    public void CreateLead_WithValidData_Succeeds()
    {
        // Arrange
        var contact = new Contact("John", "Doe", "555-1234", "john@test.com");
        
        // Act
        var lead = new Lead(contact, "Downtown", "Real Estate", "2-bed apartment", 450000);
        
        // Assert
        Assert.NotNull(lead);
        Assert.Equal("John", lead.Contact.FirstName);
        Assert.Equal(450000, lead.Price.Amount);
        Assert.Equal("Invited", lead.Status.ToString());
    }

    [Fact]
    public void CreateLead_WithPriceZero_ThrowsException()
    {
        // Arrange
        var contact = new Contact("Jane", "Smith", "555-5678", "jane@test.com");
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Lead(contact, "Uptown", "Commercial", "Office space", 0));
    }

    [Fact]
    public void CreateLead_WithNegativePrice_ThrowsException()
    {
        // Arrange
        var contact = new Contact("Bob", "Wilson", "555-9999", "bob@test.com");
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Lead(contact, "Midtown", "Development", "Land parcel", -100000));
    }

    [Fact]
    public void CreateLead_WithEmptyFirstName_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Contact("", "Doe", "555-1234", "test@test.com"));
    }

    [Fact]
    public void CreateLead_WithNullEmail_Succeeds()
    {
        // Arrange & Act
        var contact = new Contact("Alice", "Brown", "555-1111", null);
        var lead = new Lead(contact, "Downtown", "Real Estate", "Apartment", 300000);
        
        // Assert
        Assert.NotNull(lead);
        Assert.Null(lead.Contact.Email);
    }

    [Fact]
    public void Lead_AcceptStatus_UpdatesCorrectly()
    {
        // Arrange
        var contact = new Contact("Charlie", "Davis", "555-2222", "charlie@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "House", 500000);
        
        // Act
        lead.Accept();
        
        // Assert
        Assert.Equal("Accepted", lead.Status.ToString());
    }

    [Fact]
    public void Lead_DeclineStatus_UpdatesCorrectly()
    {
        // Arrange
        var contact = new Contact("Diana", "Evans", "555-3333", "diana@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "Condo", 600000);
        
        // Act
        lead.Decline();
        
        // Assert
        Assert.Equal("Declined", lead.Status.ToString());
    }
}
