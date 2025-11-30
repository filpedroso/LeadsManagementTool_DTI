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

public class GetLeadByIdQueryHandlerTests
{
    private readonly Mock<ILeadRepository> _mockRepository;
    private readonly GetLeadByIdQueryHandler _handler;

    public GetLeadByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<ILeadRepository>();
        _handler = new GetLeadByIdQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidLeadId_ShouldReturnLeadDto()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("Alice", "Johnson", "555-1111", "alice@test.com");
        var lead = new Lead(contact, "Downtown", "Residential", "House", 450000);
        
        // Use reflection to set the Id since it has a private setter
        typeof(Lead).GetProperty("Id")!.SetValue(lead, leadId);

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);

        var query = new GetLeadByIdQuery { LeadId = leadId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(leadId);
        result.ContactFirstName.Should().Be("Alice");
        result.ContactLastName.Should().Be("Johnson");
        result.ContactEmail.Should().Be("alice@test.com");
        result.ContactPhoneNumber.Should().Be("555-1111");
        result.Suburb.Should().Be("Downtown");
        result.Category.Should().Be("Residential");
        result.Description.Should().Be("House");
        result.Price.Should().Be(450000);
        result.Status.Should().Be("Invited");
    }

    [Fact]
    public async Task Handle_WithInvalidLeadId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var leadId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync((Lead?)null);

        var query = new GetLeadByIdQuery { LeadId = leadId };

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Lead with id {leadId} not found");
    }
}
