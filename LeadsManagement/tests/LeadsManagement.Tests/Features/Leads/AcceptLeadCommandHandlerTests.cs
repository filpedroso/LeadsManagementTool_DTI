namespace LeadsManagement.Tests.Features.Leads;

using Xunit;
using Moq;
using FluentAssertions;
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Infrastructure.Services;

/// <summary>
/// Testes unitários para o handler de AcceptLeadCommand
/// Usa Mocks para isolar o teste
/// </summary>
public class AcceptLeadCommandHandlerTests
{
    private readonly Mock<LeadRepository> _mockLeadRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly AcceptLeadCommandHandler _handler;

    public AcceptLeadCommandHandlerTests()
    {
        _mockLeadRepository = new Mock<LeadRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _handler = new AcceptLeadCommandHandler(_mockLeadRepository.Object, _mockEmailService.Object);
    }

    [Fact]
    public async Task Handle_WithValidLeadId_ShouldAcceptLeadAndSendEmail()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("João", email: "joao@email.com");
        var lead = new Lead(
            contact: contact,
            suburb: "São Paulo",
            category: "Tecnologia",
            description: "Software",
            price: 600); // Acima de 500, terá desconto

        _mockLeadRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);

        _mockLeadRepository.Setup(r => r.UpdateAsync(It.IsAny<Lead>()))
            .Returns(Task.CompletedTask);

        _mockLeadRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        _mockEmailService.Setup(e => e.SendLeadAcceptedNotificationAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockLeadRepository.Verify(r => r.GetByIdAsync(leadId), Times.Once);
        _mockLeadRepository.Verify(r => r.UpdateAsync(It.IsAny<Lead>()), Times.Once);
        _mockLeadRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockEmailService.Verify(e => e.SendLeadAcceptedNotificationAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>(), true), Times.Once); // true = desconto aplicado
    }

    [Fact]
    public async Task Handle_WithNonExistentLeadId_ShouldThrow()
    {
        // Arrange
        var leadId = 999;
        _mockLeadRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync((Lead)null);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldVerifyPriceDiscount()
    {
        // Arrange
        var leadId = 2;
        var contact = new Contact("Maria", email: "maria@email.com");
        var lead = new Lead(
            contact: contact,
            suburb: "Rio",
            category: "Imóvel",
            description: "Casa",
            price: 1000);

        _mockLeadRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);

        _mockLeadRepository.Setup(r => r.UpdateAsync(It.IsAny<Lead>()))
            .Returns(Task.CompletedTask);

        _mockLeadRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        _mockEmailService.Setup(e => e.SendLeadAcceptedNotificationAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // Verificar que o email foi enviado com o preço com desconto (900 = 1000 - 10%)
        _mockEmailService.Verify(e => e.SendLeadAcceptedNotificationAsync(
            It.IsAny<int>(), It.IsAny<string>(), 900m, true), Times.Once);
    }
}
