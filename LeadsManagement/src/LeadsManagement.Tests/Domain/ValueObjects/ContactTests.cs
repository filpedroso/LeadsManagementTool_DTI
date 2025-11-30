using Xunit;
using FluentAssertions;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Domain.ValueObjects;

public class ContactTests
{
    [Fact]
    public void Contact_WhenCreatedWithFirstName_ShouldSucceed()
    {
        // Act
        var contact = new Contact("John");

        // Assert
        contact.FirstName.Should().Be("John");
        contact.LastName.Should().BeNull();
        contact.PhoneNumber.Should().BeNull();
        contact.Email.Should().BeNull();
    }

    [Fact]
    public void Contact_WhenCreatedWithAllFields_ShouldSetAllProperties()
    {
        // Act
        var contact = new Contact("John", "Doe", "555-1234", "john@test.com");

        // Assert
        contact.FirstName.Should().Be("John");
        contact.LastName.Should().Be("Doe");
        contact.PhoneNumber.Should().Be("555-1234");
        contact.Email.Should().Be("john@test.com");
    }

    [Fact]
    public void Contact_WhenFirstNameIsNull_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new Contact(null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("First name is required*")
            .WithParameterName("firstName");
    }

    [Fact]
    public void Contact_WhenFirstNameIsWhitespace_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new Contact("   ");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("First name is required*")
            .WithParameterName("firstName");
    }

    [Fact]
    public void FullName_WhenLastNameIsNull_ShouldReturnFirstNameOnly()
    {
        // Arrange
        var contact = new Contact("John");

        // Act
        var fullName = contact.FullName;

        // Assert
        fullName.Should().Be("John");
    }

    [Fact]
    public void FullName_WhenLastNameIsEmpty_ShouldReturnFirstNameOnly()
    {
        // Arrange
        var contact = new Contact("John", "");

        // Act
        var fullName = contact.FullName;

        // Assert
        fullName.Should().Be("John");
    }

    [Fact]
    public void FullName_WhenLastNameExists_ShouldReturnFullName()
    {
        // Arrange
        var contact = new Contact("John", "Doe");

        // Act
        var fullName = contact.FullName;

        // Assert
        fullName.Should().Be("John Doe");
    }
}
