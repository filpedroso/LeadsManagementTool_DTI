using Xunit;
using Moq;
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Leads.Commands;

public class CreateLeadCommandHandlerTests
{
    private readonly Mock<LeadRepository> _mockRepository;
    private readonly CreateLeadCommandHandler _handler;

    public CreateLeadCommandHandlerTests()
    {
        _mockRepository = new Mock<LeadRepository>();
        _handler = new CreateLeadCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidLead_ReturnsLeadId()
    {
        // Arrange
        var command = new CreateLeadCommand
        {
            ContactFirstName = "John",
            ContactLastName = "Doe",
            ContactEmail = "john@test.com",
            ContactPhoneNumber = "555-1234",
            Suburb = "Downtown",
            Category = "Real Estate",
            Description = "2-bedroom apartment",
            Price = 450000
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Lead>())).Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Lead>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithPriceZero_ThrowsException()
    {
        // Arrange
        var command = new CreateLeadCommand
        {
            ContactFirstName = "Jane",
            ContactLastName = "Smith",
            ContactEmail = "jane@test.com",
            ContactPhoneNumber = "555-5678",
            Suburb = "Uptown",
            Category = "Commercial",
            Description = "Office space",
            Price = 0
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithEmptyFirstName_ThrowsException()
    {
        // Arrange
        var command = new CreateLeadCommand
        {
            ContactFirstName = "",
            ContactLastName = "Wilson",
            ContactEmail = "test@test.com",
            ContactPhoneNumber = "555-9999",
            Suburb = "Midtown",
            Category = "Development",
            Description = "Land parcel",
            Price = 750000
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
