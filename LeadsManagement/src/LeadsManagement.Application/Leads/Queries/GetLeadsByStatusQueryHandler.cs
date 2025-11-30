namespace LeadsManagement.Application.Features.Leads.Queries;

using MediatR;
using LeadsManagement.Application.Features.Leads.DTOs;
using LeadsManagement.Domain.Enums;
using LeadsManagement.Infrastructure.Data.Repositories;
using Mapster;

/// <summary>
/// Handler que processa GetLeadsByStatusQuery
/// Busca todos os leads com um status específico
/// </summary>
public class GetLeadsByStatusQueryHandler : IRequestHandler<GetLeadsByStatusQuery, List<LeadDto>>
{
    private readonly ILeadRepository _leadRepository;

    public GetLeadsByStatusQueryHandler(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<List<LeadDto>> Handle(GetLeadsByStatusQuery request, CancellationToken cancellationToken)
    {
        // Parsear status
        if (!Enum.TryParse<LeadStatus>(request.Status, true, out var status))
            throw new ArgumentException($"Invalid status: {request.Status}");

        // Buscar leads
        var leads = await _leadRepository.GetLeadsByStatusAsync(status);

        // Mapear para DTOs
        var dtos = leads
            .Adapt<List<LeadDto>>()
            .ToList();

        return dtos;
    }
}
