namespace LeadsManagement.Application.Features.Leads.DTOs;

/// <summary>
/// DTO para criar um novo Lead
/// Recebido do frontend
/// </summary>
public class CreateLeadDto
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
