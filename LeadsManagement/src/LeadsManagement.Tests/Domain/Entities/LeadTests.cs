using Xunit;
using FluentAssertions;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Domain.Enums;

namespace LeadsManagement.Tests.Domain.Entities;

public class LeadTests
{
    [Fact]
    public void Lead_WhenCreated_ShouldHaveInvitedStatus()
    {
        // Arrange
        var contact = new Contact("John", "Doe", "555-1234", "john@test.com");

        // Act
        var lead = new Lead(contact, "Downtown", "Real Estate", "Apartment", 400000);

        // Assert
        lead.Status.Should().Be(LeadStatus.Invited);
        lead.Contact.Should().Be(contact);
        lead.Suburb.Should().Be("Downtown");
        lead.Category.Should().Be("Real Estate");
        lead.Description.Should().Be("Apartment");
        lead.Price.Amount.Should().Be(400000);
        lead.DateCreated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Lead_WhenCreatedWithNullContact_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new Lead(null!, "Downtown", "Real Estate", "Apartment", 400000);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("contact");
    }

    [Fact]
    public void Accept_WhenLeadIsInvited_ShouldChangeStatusToAccepted()
    {
        // Arrange
        var contact = new Contact("Alice", "Smith");
        var lead = new Lead(contact, "Suburb", "Category", "Description", 300);

        // Act
        lead.Accept();

        // Assert
        lead.Status.Should().Be(LeadStatus.Accepted);
    }

    [Fact]
    public void Accept_WhenPriceAbove500_ShouldApply10PercentDiscount()
    {
        // Arrange
        var contact = new Contact("Bob", "Jones");
        var lead = new Lead(contact, "Suburb", "Category", "Description", 1000);

        // Act
        lead.Accept();

        // Assert
        lead.Price.Amount.Should().Be(900); // 10% discount applied
        lead.Status.Should().Be(LeadStatus.Accepted);
    }

    [Fact]
    public void Accept_WhenPriceIs500OrLess_ShouldNotApplyDiscount()
    {
        // Arrange
        var contact = new Contact("Charlie", "Brown");
        var lead = new Lead(contact, "Suburb", "Category", "Description", 500);

        // Act
        lead.Accept();

        // Assert
        lead.Price.Amount.Should().Be(500); // No discount
        lead.Status.Should().Be(LeadStatus.Accepted);
    }

    [Fact]
    public void Accept_WhenLeadIsNotInvited_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var contact = new Contact("Diana", "Prince");
        var lead = new Lead(contact, "Suburb", "Category", "Description", 600);
        lead.Accept(); // Now status is Accepted

        // Act
        Action act = () => lead.Accept();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot accept a lead with status Accepted");
    }

    [Fact]
    public void Decline_WhenLeadIsInvited_ShouldChangeStatusToDeclined()
    {
        // Arrange
        var contact = new Contact("Eve", "Adams");
        var lead = new Lead(contact, "Suburb", "Category", "Description", 300);

        // Act
        lead.Decline();

        // Assert
        lead.Status.Should().Be(LeadStatus.Declined);
    }

    [Fact]
    public void Decline_WhenLeadIsNotInvited_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var contact = new Contact("Frank", "Miller");
        var lead = new Lead(contact, "Suburb", "Category", "Description", 600);
        lead.Decline(); // Now status is Declined

        // Act
        Action act = () => lead.Decline();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot decline a lead with status Declined");
    }
}
