using Xunit;
using Moq;
using LeadsManagement.Application.Features.Leads.Queries;
using LeadsManagement.Application.Features.Leads.DTOs;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Leads.Queries;

public class GetLeadByIdQueryHandlerTests
{
    private readonly Mock<LeadRepository> _mockRepository;
    private readonly GetLeadByIdQueryHandler _handler;

    public GetLeadByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<LeadRepository>();
        _handler = new GetLeadByIdQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ReturnsLeadDto()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("John", "Doe", "555-1234", "john@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "Apartment", 450000);
        
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);

        var query = new GetLeadByIdQuery { LeadId = leadId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<LeadDto>(result);
        _mockRepository.Verify(r => r.GetByIdAsync(leadId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var leadId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync((Lead?)null);

        var query = new GetLeadByIdQuery { LeadId = leadId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithZeroId_ThrowsException()
    {
        // Arrange
        var query = new GetLeadByIdQuery { LeadId = 0 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }
}
