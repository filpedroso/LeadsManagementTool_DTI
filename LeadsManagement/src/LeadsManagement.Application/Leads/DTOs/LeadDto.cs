#nullable disable

namespace LeadsManagement.Application.Features.Leads.DTOs;

/// <summary>
/// DTO para representar um Lead na API
/// Usado para retornar dados de lead
/// </summary>
public class LeadDto
{
    public int Id { get; set; }
    public string ContactFirstName { get; set; }
    public string ContactLastName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhoneNumber { get; set; }
    public DateTime DateCreated { get; set; }
    public string Suburb { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
}
