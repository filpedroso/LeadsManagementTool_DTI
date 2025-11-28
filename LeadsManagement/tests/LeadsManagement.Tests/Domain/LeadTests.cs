namespace LeadsManagement.Tests.Domain;

using Xunit;
using FluentAssertions;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.Enums;
using LeadsManagement.Domain.ValueObjects;


// Testes unitários para a entidade Lead
public class LeadTests
{
    [Fact]
    public void CreateLead_WithValidData_ShouldSucceed()
    {
        var contact = new Contact("Carla", "Jones", "31999999999", "carla@email.com");
        
        var lead = new Lead(
            contact: contact,
            suburb: "São Paulo",
            category: "Tecnologia",
            description: "Venda de software",
            price: 1000);

        // Assert
        lead.Should().NotBeNull();
        lead.Contact.FirstName.Should().Be("Carla");
        lead.Status.Should().Be(LeadStatus.Invited);
        lead.Price.Amount.Should().Be(1000);
        lead.DateCreated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateLead_WithNullContact_ShouldThrow()
    {
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
        var contact = new Contact("Maria", "Bebete");
        var lead = new Lead(
            contact: contact,
            suburb: "Rio de Janeiro",
            category: "Imóvel",
            description: "Propriedade comercial",
            price: 1000);

        lead.Accept();

        lead.Status.Should().Be(LeadStatus.Accepted);
        lead.Price.Amount.Should().Be(900);
    }

    [Fact]
    public void Accept_WhenPriceBelow500_ShouldNotApplyDiscount()
    {
        var contact = new Contact("Pedro");
        var lead = new Lead(
            contact: contact,
            suburb: "Brasília",
            category: "Serviços",
            description: "Consultoria",
            price: 300);

        lead.Accept();

        lead.Status.Should().Be(LeadStatus.Accepted);
        lead.Price.Amount.Should().Be(300);
    }

    [Fact]
    public void Decline_ShouldChangeStatus()
    {
        var contact = new Contact("Carlos");
        var lead = new Lead(
            contact: contact,
            suburb: "Curitiba",
            category: "Alimentos",
            description: "Restaurante",
            price: 500);

        lead.Decline();
        lead.Status.Should().Be(LeadStatus.Declined);
    }

    [Fact]
    public void Accept_WhenAlreadyAccepted_ShouldThrow()
    {
        var contact = new Contact("Fernanda");
        var lead = new Lead(
            contact: contact,
            suburb: "Recife",
            category: "Moda",
            description: "Loja de roupas",
            price: 550);

        lead.Accept();

        var action = () => lead.Accept();
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Decline_WhenAlreadyDeclined_ShouldThrow()
    {
        var contact = new Contact("Roberto");
        var lead = new Lead(
            contact: contact,
            suburb: "Fortaleza",
            category: "Saúde",
            description: "Clínica",
            price: 800);

        lead.Decline();

        var action = () => lead.Decline();
        action.Should().Throw<InvalidOperationException>();
    }
}
