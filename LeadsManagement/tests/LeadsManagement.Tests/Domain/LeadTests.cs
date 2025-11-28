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
