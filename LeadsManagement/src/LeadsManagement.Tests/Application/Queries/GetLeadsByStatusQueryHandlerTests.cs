using Xunit;
using FluentAssertions;
using Moq;
using LeadsManagement.Application.Features.Leads.Queries;
using LeadsManagement.Application.Features.Leads.DTOs;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Domain.Enums;

namespace LeadsManagement.Tests.Application.Queries;

public class GetLeadsByStatusQueryHandlerTests
{
    private readonly Mock<ILeadRepository> _mockRepository;
    private readonly GetLeadsByStatusQueryHandler _handler;

    public GetLeadsByStatusQueryHandlerTests()
    {
        _mockRepository = new Mock<ILeadRepository>();
        _handler = new GetLeadsByStatusQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidStatus_ShouldReturnLeadDtoList()
    {
        // Arrange
        var contact1 = new Contact("Bob", "Smith", "555-2222", "bob@test.com");
        var contact2 = new Contact("Carol", "White", "555-3333", "carol@test.com");
        
        var lead1 = new Lead(contact1, "Uptown", "Commercial", "Office", 800000);
        var lead2 = new Lead(contact2, "Midtown", "Residential", "Apartment", 350000);

        // Set IDs via reflection
        typeof(Lead).GetProperty("Id")!.SetValue(lead1, 1);
        typeof(Lead).GetProperty("Id")!.SetValue(lead2, 2);

        var leads = new List<Lead> { lead1, lead2 };

        _mockRepository.Setup(r => r.GetLeadsByStatusAsync(LeadStatus.Invited))
            .ReturnsAsync(leads);

        var query = new GetLeadsByStatusQuery { Status = "Invited" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        
        result[0].Id.Should().Be(1);
        result[0].ContactFirstName.Should().Be("Bob");
        result[0].Status.Should().Be("Invited");
        
        result[1].Id.Should().Be(2);
        result[1].ContactFirstName.Should().Be("Carol");
        result[1].Status.Should().Be("Invited");
    }

    [Fact]
    public async Task Handle_WithAcceptedStatus_ShouldReturnAcceptedLeads()
    {
        // Arrange
        var contact = new Contact("Dave", "Green");
        var lead = new Lead(contact, "Suburb", "Category", "Description", 600);
        lead.Accept();

        typeof(Lead).GetProperty("Id")!.SetValue(lead, 3);

        var leads = new List<Lead> { lead };

        _mockRepository.Setup(r => r.GetLeadsByStatusAsync(LeadStatus.Accepted))
            .ReturnsAsync(leads);

        var query = new GetLeadsByStatusQuery { Status = "Accepted" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Status.Should().Be("Accepted");
        result[0].Price.Should().Be(540); // 10% discount applied
    }

    [Fact]
    public async Task Handle_WithDeclinedStatus_ShouldReturnDeclinedLeads()
    {
        // Arrange
        var contact = new Contact("Emily", "Black");
        var lead = new Lead(contact, "Suburb", "Category", "Description", 400);
        lead.Decline();

        typeof(Lead).GetProperty("Id")!.SetValue(lead, 4);

        var leads = new List<Lead> { lead };

        _mockRepository.Setup(r => r.GetLeadsByStatusAsync(LeadStatus.Declined))
            .ReturnsAsync(leads);

        var query = new GetLeadsByStatusQuery { Status = "Declined" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Status.Should().Be("Declined");
    }

    [Fact]
    public async Task Handle_WithInvalidStatus_ShouldThrowArgumentException()
    {
        // Arrange
        var query = new GetLeadsByStatusQuery { Status = "InvalidStatus" };

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid status: InvalidStatus");
    }

    [Fact]
    public async Task Handle_WithNoLeadsForStatus_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetLeadsByStatusAsync(LeadStatus.Invited))
            .ReturnsAsync(new List<Lead>());

        var query = new GetLeadsByStatusQuery { Status = "Invited" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("invited")]
    [InlineData("INVITED")]
    [InlineData("Invited")]
    public async Task Handle_WithCaseInsensitiveStatus_ShouldWork(string status)
    {
        // Arrange
        _mockRepository.Setup(r => r.GetLeadsByStatusAsync(LeadStatus.Invited))
            .ReturnsAsync(new List<Lead>());

        var query = new GetLeadsByStatusQuery { Status = status };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }
}
