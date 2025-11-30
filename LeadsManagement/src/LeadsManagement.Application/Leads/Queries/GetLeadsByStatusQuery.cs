#nullable disable

namespace LeadsManagement.Application.Features.Leads.Queries;

using MediatR;
using LeadsManagement.Application.Features.Leads.DTOs;

/// <summary>
/// Query para buscar leads por status
/// Implementa padrão CQRS - Query
/// </summary>
public class GetLeadsByStatusQuery : IRequest<List<LeadDto>>
{
    public string Status { get; set; }
}
