namespace LeadsManagement.Application.Features.Leads.Queries;

using MediatR;
using LeadsManagement.Application.Features.Leads.DTOs;
using LeadsManagement.Infrastructure.Data.Repositories;
using Mapster;

/// <summary>
/// Handler que processa GetLeadByIdQuery
/// Busca um lead específico pelo ID
/// </summary>
public class GetLeadByIdQueryHandler : IRequestHandler<GetLeadByIdQuery, LeadDto>
{
    private readonly LeadRepository _leadRepository;

    public GetLeadByIdQueryHandler(LeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<LeadDto> Handle(GetLeadByIdQuery request, CancellationToken cancellationToken)
    {
        // Buscar lead
        var lead = await _leadRepository.GetByIdAsync(request.LeadId);
        if (lead == null)
            throw new InvalidOperationException($"Lead with id {request.LeadId} not found");

        // Mapear para DTO
        return lead.Adapt<LeadDto>();
    }
}
