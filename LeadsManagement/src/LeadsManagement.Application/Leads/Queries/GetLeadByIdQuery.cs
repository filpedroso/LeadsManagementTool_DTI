namespace LeadsManagement.Application.Features.Leads.Queries;

using MediatR;
using LeadsManagement.Application.Features.Leads.DTOs;

/// <summary>
/// Query para buscar um lead específico por ID
/// </summary>
public class GetLeadByIdQuery : IRequest<LeadDto>
{
    public int LeadId { get; set; }
}
