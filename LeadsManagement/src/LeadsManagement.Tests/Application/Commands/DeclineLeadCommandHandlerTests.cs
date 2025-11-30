using Xunit;
using FluentAssertions;
using Moq;
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Domain.Enums;

namespace LeadsManagement.Tests.Application.Commands;

public class DeclineLeadCommandHandlerTests
{
    private readonly Mock<ILeadRepository> _mockRepository;
    private readonly DeclineLeadCommandHandler _handler;

    public DeclineLeadCommandHandlerTests()
    {
        _mockRepository = new Mock<ILeadRepository>();
        _handler = new DeclineLeadCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidLead_ShouldDeclineAndSave()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("Eve", "Martinez", "555-4321", "eve@test.com");
        var lead = new Lead(contact, "Uptown", "Commercial", "Office", 750000);

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Lead>()))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        lead.Status.Should().Be(LeadStatus.Declined);
        _mockRepository.Verify(r => r.UpdateAsync(lead), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidLeadId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var leadId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync((Lead?)null);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Lead with id {leadId} not found");
    }

    [Fact]
    public async Task Handle_WithAlreadyDeclinedLead_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var leadId = 2;
        var contact = new Contact("Frank", "Lopez");
        var lead = new Lead(contact, "Suburb", "Category", "Description", 400);
        lead.Decline(); // Already declined

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot decline a lead with status Declined");
    }

    [Fact]
    public async Task Handle_WithAcceptedLead_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var leadId = 3;
        var contact = new Contact("Grace", "Kim");
        var lead = new Lead(contact, "Suburb", "Category", "Description", 600);
        lead.Accept(); // Now it's Accepted

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot decline a lead with status Accepted");
    }
}
