using Xunit;
using Moq;
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Services;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Leads.Commands;

public class AcceptLeadCommandHandlerTests
{
    private readonly Mock<LeadRepository> _mockRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly AcceptLeadCommandHandler _handler;

    public AcceptLeadCommandHandlerTests()
    {
        _mockRepository = new Mock<LeadRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _handler = new AcceptLeadCommandHandler(_mockRepository.Object, _mockEmailService.Object);
    }

    [Fact]
    public async Task Handle_WithValidLead_AcceptsAndSaves()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("John", "Doe", "555-1234", "john@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "Apartment", 450000);

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Accepted", lead.Status.ToString());
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidLeadId_ThrowsException()
    {
        // Arrange
        var leadId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync((Lead?)null);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithAlreadyAcceptedLead_UpdatesStatus()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("Jane", "Smith", "555-5678", "jane@test.com");
        var lead = new Lead(contact, "Uptown", "Commercial", "Office", 600000);
        lead.Accept();

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Accepted", lead.Status.ToString());
    }
}
