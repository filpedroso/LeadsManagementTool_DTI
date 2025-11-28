namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;

/// <summary>
/// Command para criar um novo Lead
/// Implementa padrão CQRS - Command
/// </summary>
public class CreateLeadCommand : IRequest<int>
{
    public string ContactFirstName { get; set; }
    public string? ContactLastName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhoneNumber { get; set; }
    public string Suburb { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}
