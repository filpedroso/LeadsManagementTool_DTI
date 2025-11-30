using Xunit;
using FluentAssertions;
using Moq;
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Application.Commands;

public class CreateLeadCommandHandlerTests
{
    private readonly Mock<ILeadRepository> _mockRepository;
    private readonly CreateLeadCommandHandler _handler;

    public CreateLeadCommandHandlerTests()
    {
        _mockRepository = new Mock<ILeadRepository>();
        _handler = new CreateLeadCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateLeadAndReturnId()
    {
        // Arrange
        var command = new CreateLeadCommand
        {
            ContactFirstName = "John",
            ContactLastName = "Doe",
            ContactPhoneNumber = "555-1234",
            ContactEmail = "john@test.com",
            Suburb = "Downtown",
            Category = "Real Estate",
            Description = "Apartment sale",
            Price = 500000
        };

        Lead? capturedLead = null;
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Lead>()))
            .Callback<Lead>(lead => capturedLead = lead)
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Lead>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

        capturedLead.Should().NotBeNull();
        capturedLead!.Contact.FirstName.Should().Be("John");
        capturedLead.Contact.LastName.Should().Be("Doe");
        capturedLead.Contact.PhoneNumber.Should().Be("555-1234");
        capturedLead.Contact.Email.Should().Be("john@test.com");
        capturedLead.Suburb.Should().Be("Downtown");
        capturedLead.Category.Should().Be("Real Estate");
        capturedLead.Description.Should().Be("Apartment sale");
        capturedLead.Price.Amount.Should().Be(500000);
    }

    [Fact]
    public async Task Handle_WithZeroPrice_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateLeadCommand
        {
            ContactFirstName = "Jane",
            ContactLastName = "Smith",
            Suburb = "Suburb",
            Category = "Category",
            Description = "Description",
            Price = 0
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Price must be greater than zero");
    }

    [Fact]
    public async Task Handle_WithNegativePrice_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateLeadCommand
        {
            ContactFirstName = "Bob",
            Suburb = "Suburb",
            Category = "Category",
            Description = "Description",
            Price = -100
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Price must be greater than zero");
    }
}
