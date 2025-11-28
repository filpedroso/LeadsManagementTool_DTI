namespace LeadsManagement.Tests.Features.Leads;

using Xunit;
using Moq;
using FluentAssertions;
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Testes unitários para o handler de DeclineLeadCommand
/// </summary>
public class DeclineLeadCommandHandlerTests
{
    private readonly Mock<LeadRepository> _mockLeadRepository;
    private readonly DeclineLeadCommandHandler _handler;

    public DeclineLeadCommandHandlerTests()
    {
        _mockLeadRepository = new Mock<LeadRepository>();
        _handler = new DeclineLeadCommandHandler(_mockLeadRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidLeadId_ShouldDeclineLead()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("Pedro");
        var lead = new Lead(
            contact: contact,
            suburb: "Brasília",
            category: "Serviços",
            description: "Consultoria",
            price: 400);

        _mockLeadRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);

        _mockLeadRepository.Setup(r => r.UpdateAsync(It.IsAny<Lead>()))
            .Returns(Task.CompletedTask);

        _mockLeadRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockLeadRepository.Verify(r => r.GetByIdAsync(leadId), Times.Once);
        _mockLeadRepository.Verify(r => r.UpdateAsync(It.IsAny<Lead>()), Times.Once);
        _mockLeadRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentLeadId_ShouldThrow()
    {
        // Arrange
        var leadId = 999;
        _mockLeadRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync((Lead)null);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
