namespace LeadsManagement.Tests.Features.Leads;

using Xunit;
using Moq;
using FluentAssertions;
using LeadsManagement.Application.Features.Leads.DTOs;
using LeadsManagement.Application.Features.Leads.Queries;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.Enums;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Testes unitários para o handler de GetLeadsByStatusQuery
/// </summary>
public class GetLeadsByStatusQueryHandlerTests
{
    private readonly Mock<LeadRepository> _mockLeadRepository;
    private readonly GetLeadsByStatusQueryHandler _handler;

    public GetLeadsByStatusQueryHandlerTests()
    {
        _mockLeadRepository = new Mock<LeadRepository>();
        _handler = new GetLeadsByStatusQueryHandler(_mockLeadRepository.Object);
    }

    [Fact]
    public async Task Handle_WithInvitedStatus_ShouldReturnLeadsWithInvitedStatus()
    {
        // Arrange
        var contact1 = new Contact("Ana");
        var lead1 = new Lead(contact1, "Salvador", "Educação", "Curso", 300);

        var contact2 = new Contact("Bruno");
        var lead2 = new Lead(contact2, "Fortaleza", "Saúde", "Clínica", 250);

        var leads = new List<Lead> { lead1, lead2 };

        _mockLeadRepository.Setup(r => r.GetLeadsByStatusAsync(LeadStatus.Invited))
            .ReturnsAsync(leads);

        var query = new GetLeadsByStatusQuery { Status = "Invited" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        _mockLeadRepository.Verify(r => r.GetLeadsByStatusAsync(LeadStatus.Invited), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidStatus_ShouldThrow()
    {
        // Arrange
        var query = new GetLeadsByStatusQuery { Status = "InvalidStatus" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNoLeads_ShouldReturnEmptyList()
    {
        // Arrange
        _mockLeadRepository.Setup(r => r.GetLeadsByStatusAsync(LeadStatus.Accepted))
            .ReturnsAsync(new List<Lead>());

        var query = new GetLeadsByStatusQuery { Status = "Accepted" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
