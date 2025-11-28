// ===================================================================
// FILE: tests/LeadsManagement.Tests/Domain/LeadTests.cs
// ===================================================================

namespace LeadsManagement.Tests.Domain;

using Xunit;
using FluentAssertions;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.Enums;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Domain.Events;

/// <summary>
/// Testes unitários para a entidade Lead
/// Testa regras de negócio e validações
/// </summary>
public class LeadTests
{
    [Fact]
    public void CreateLead_WithValidData_ShouldSucceed()
    {
        // Arrange
        var contact = new Contact("João", "Silva", "11999999999", "joao@email.com");
        
        // Act
        var lead = new Lead(
            contact: contact,
            suburb: "São Paulo",
            category: "Tecnologia",
            description: "Venda de software",
            price: 1000);

        // Assert
        lead.Should().NotBeNull();
        lead.Contact.FirstName.Should().Be("João");
        lead.Status.Should().Be(LeadStatus.Invited);
        lead.Price.Amount.Should().Be(1000);
        lead.DateCreated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateLead_WithNullContact_ShouldThrow()
    {
        // Act & Assert
        var action = () => new Lead(
            contact: null,
            suburb: "São Paulo",
            category: "Tecnologia",
            description: "Venda de software",
            price: 1000);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Accept_WhenPriceAbove500_ShouldApplyDiscount()
    {
        // Arrange
        var contact = new Contact("Maria", "Santos");
        var lead = new Lead(
            contact: contact,
            suburb: "Rio de Janeiro",
            category: "Imóvel",
            description: "Propriedade comercial",
            price: 1000); // Preço acima de $500

        // Act
        lead.Accept();

        // Assert
        lead.Status.Should().Be(LeadStatus.Accepted);
        lead.Price.Amount.Should().Be(900); // 1000 - 10% = 900
    }

    [Fact]
    public void Accept_WhenPriceBelow500_ShouldNotApplyDiscount()
    {
        // Arrange
        var contact = new Contact("Pedro");
        var lead = new Lead(
            contact: contact,
            suburb: "Brasília",
            category: "Serviços",
            description: "Consultoria",
            price: 300); // Preço abaixo de $500

        // Act
        lead.Accept();

        // Assert
        lead.Status.Should().Be(LeadStatus.Accepted);
        lead.Price.Amount.Should().Be(300); // Sem desconto
    }

    [Fact]
    public void Accept_ShouldRaiseDomainEvent()
    {
        // Arrange
        var contact = new Contact("Ana");
        var lead = new Lead(
            contact: contact,
            suburb: "Salvador",
            category: "Educação",
            description: "Curso online",
            price: 600);

        // Act
        lead.Accept();

        // Assert
        lead.DomainEvents.Should().HaveCount(1);
        lead.DomainEvents.First().Should().BeOfType<LeadAcceptedEvent>();
    }

    [Fact]
    public void Decline_ShouldChangeStatus()
    {
        // Arrange
        var contact = new Contact("Carlos");
        var lead = new Lead(
            contact: contact,
            suburb: "Curitiba",
            category: "Alimentos",
            description: "Restaurante",
            price: 500);

        // Act
        lead.Decline();

        // Assert
        lead.Status.Should().Be(LeadStatus.Declined);
    }

    [Fact]
    public void Decline_ShouldRaiseDomainEvent()
    {
        // Arrange
        var contact = new Contact("Lucas");
        var lead = new Lead(
            contact: contact,
            suburb: "Manaus",
            category: "Turismo",
            description: "Agência de viagens",
            price: 750);

        // Act
        lead.Decline();

        // Assert
        lead.DomainEvents.Should().HaveCount(1);
        lead.DomainEvents.First().Should().BeOfType<LeadDeclinedEvent>();
    }

    [Fact]
    public void Accept_WhenAlreadyAccepted_ShouldThrow()
    {
        // Arrange
        var contact = new Contact("Fernanda");
        var lead = new Lead(
            contact: contact,
            suburb: "Recife",
            category: "Moda",
            description: "Loja de roupas",
            price: 550);
        
        lead.Accept();

        // Act & Assert
        var action = () => lead.Accept();
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Decline_WhenAlreadyDeclined_ShouldThrow()
    {
        // Arrange
        var contact = new Contact("Roberto");
        var lead = new Lead(
            contact: contact,
            suburb: "Fortaleza",
            category: "Saúde",
            description: "Clínica",
            price: 800);
        
        lead.Decline();

        // Act & Assert
        var action = () => lead.Decline();
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var contact = new Contact("Beatriz");
        var lead = new Lead(
            contact: contact,
            suburb: "Porto Alegre",
            category: "Financeiro",
            description: "Consultoria financeira",
            price: 900);
        
        lead.Accept();
        lead.DomainEvents.Should().HaveCount(1);

        // Act
        lead.ClearDomainEvents();

        // Assert
        lead.DomainEvents.Should().HaveCount(0);
    }
}

// ===================================================================
// FILE: tests/LeadsManagement.Tests/Features/Leads/AcceptLeadCommandHandlerTests.cs
// ===================================================================

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

// ===================================================================
// FILE: tests/LeadsManagement.Tests/Features/Leads/DeclineLeadCommandHandlerTests.cs
// ===================================================================

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

// ===================================================================
// FILE: tests/LeadsManagement.Tests/Features/Leads/GetLeadsByStatusQueryHandlerTests.cs
// ===================================================================

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
