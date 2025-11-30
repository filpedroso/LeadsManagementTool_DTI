using Xunit;
using FluentAssertions;
using Moq;
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Domain.Services;
using LeadsManagement.Domain.Enums;

namespace LeadsManagement.Tests.Application.Commands;

public class AcceptLeadCommandHandlerTests
{
    private readonly Mock<ILeadRepository> _mockRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly AcceptLeadCommandHandler _handler;

    public AcceptLeadCommandHandlerTests()
    {
        _mockRepository = new Mock<ILeadRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _handler = new AcceptLeadCommandHandler(_mockRepository.Object, _mockEmailService.Object);
    }

    [Fact]
    public async Task Handle_WithValidLead_ShouldAcceptAndSave()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("Alice", "Brown", "555-5678", "alice@test.com");
        var lead = new Lead(contact, "Suburb", "Category", "Description", 300);

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Lead>()))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        _mockEmailService.Setup(e => e.SendLeadAcceptedNotificationAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        lead.Status.Should().Be(LeadStatus.Accepted);
        lead.Price.Amount.Should().Be(300); // No discount (price <= 500)
        _mockRepository.Verify(r => r.UpdateAsync(lead), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockEmailService.Verify(e => e.SendLeadAcceptedNotificationAsync(
            It.IsAny<int>(), "vendas@test.com", 300, false), Times.Once);
    }

    [Fact]
    public async Task Handle_WithPriceAbove500_ShouldApplyDiscountAndNotify()
    {
        // Arrange
        var leadId = 2;
        var contact = new Contact("Bob", "Wilson", "555-9999", "bob@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "Condo", 1000);

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Lead>()))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        _mockEmailService.Setup(e => e.SendLeadAcceptedNotificationAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        lead.Status.Should().Be(LeadStatus.Accepted);
        lead.Price.Amount.Should().Be(900); // 10% discount applied
        _mockRepository.Verify(r => r.UpdateAsync(lead), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockEmailService.Verify(e => e.SendLeadAcceptedNotificationAsync(
            It.IsAny<int>(), "vendas@test.com", 900, true), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidLeadId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var leadId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync((Lead?)null);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Lead with id {leadId} not found");
    }

    [Fact]
    public async Task Handle_WithAlreadyAcceptedLead_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var leadId = 3;
        var contact = new Contact("Charlie", "Davis");
        var lead = new Lead(contact, "Suburb", "Category", "Description", 600);
        lead.Accept(); // Already accepted

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot accept a lead with status Accepted");
    }
}
