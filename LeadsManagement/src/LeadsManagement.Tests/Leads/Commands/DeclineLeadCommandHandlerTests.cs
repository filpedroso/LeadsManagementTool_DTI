using Xunit;
using Moq;
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;
namespace LeadsManagement.Tests.Leads.Commands;

public class DeclineLeadCommandHandlerTests
{
    private readonly Mock<LeadRepository> _mockRepository;
    private readonly DeclineLeadCommandHandler _handler;

    public DeclineLeadCommandHandlerTests()
    {
        _mockRepository = new Mock<LeadRepository>();
        _handler = new DeclineLeadCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidLead_DeclinesAndSaves()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("Bob", "Wilson", "555-9999", "bob@test.com");
        var lead = new Lead(contact, "Midtown", "Development", "Land", 750000);

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Declined", lead.Status.ToString());
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidLeadId_ThrowsException()
    {
        // Arrange
        var leadId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync((Lead?)null);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CanDeclineAcceptedLead()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("Alice", "Brown", "555-1111", "alice@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "Condo", 500000);
        lead.Accept();

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Declined", lead.Status.ToString());
    }
}
