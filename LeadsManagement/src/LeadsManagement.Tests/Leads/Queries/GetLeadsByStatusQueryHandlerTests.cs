using Xunit;
using Moq;
using LeadsManagement.Application.Features.Leads.Queries;
using LeadsManagement.Application.Features.Leads.DTOs;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.Enums;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Leads.Queries;

public class GetLeadsByStatusQueryHandlerTests
{
    private readonly Mock<LeadRepository> _mockRepository;
    private readonly GetLeadsByStatusQueryHandler _handler;

    public GetLeadsByStatusQueryHandlerTests()
    {
        _mockRepository = new Mock<LeadRepository>();
        _handler = new GetLeadsByStatusQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithInvitedStatus_ReturnsLeads()
    {
        // Arrange
        var leads = new List<Lead>
        {
            new Lead(
                new Contact("John", "Doe", "555-1234", "john@test.com"),
                "Downtown", "Real Estate", "Apartment", 450000),
            new Lead(
                new Contact("Jane", "Smith", "555-5678", "jane@test.com"),
                "Uptown", "Commercial", "Office", 600000)
        };

        _mockRepository.Setup(r => r.GetLeadsByStatusAsync(LeadStatus.Invited))
            .ReturnsAsync(leads);

        var query = new GetLeadsByStatusQuery { Status = "Invited" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _mockRepository.Verify(r => r.GetLeadsByStatusAsync(LeadStatus.Invited), Times.Once);
    }

    [Fact]
    public async Task Handle_WithAcceptedStatus_ReturnsOnlyAccepted()
    {
        // Arrange
        var leads = new List<Lead>();
        _mockRepository.Setup(r => r.GetLeadsByStatusAsync(LeadStatus.Accepted))
            .ReturnsAsync(leads);

        var query = new GetLeadsByStatusQuery { Status = "Accepted" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithInvalidStatus_ThrowsException()
    {
        // Arrange
        var query = new GetLeadsByStatusQuery { Status = "InvalidStatus" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Theory]
    [InlineData("Invited")]
    [InlineData("Accepted")]
    [InlineData("Declined")]
    public async Task Handle_WithValidStatuses_Succeeds(string statusString)
    {
        // Arrange
        var status = Enum.Parse<LeadStatus>(statusString);
        _mockRepository.Setup(r => r.GetLeadsByStatusAsync(status))
            .ReturnsAsync(new List<Lead>());

        var query = new GetLeadsByStatusQuery { Status = statusString };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.GetLeadsByStatusAsync(status), Times.Once);
    }
}
