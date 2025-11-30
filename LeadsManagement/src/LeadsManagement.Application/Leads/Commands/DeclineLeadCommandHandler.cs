namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;
using LeadsManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Handler que processa DeclineLeadCommand
/// Recusa um lead
/// </summary>
public class DeclineLeadCommandHandler : IRequestHandler<DeclineLeadCommand, Unit>
{
    private readonly LeadRepository _leadRepository;

    public DeclineLeadCommandHandler(LeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<Unit> Handle(DeclineLeadCommand request, CancellationToken cancellationToken)
    {
        // Buscar lead
        var lead = await _leadRepository.GetByIdAsync(request.LeadId);
        if (lead == null)
            throw new InvalidOperationException($"Lead with id {request.LeadId} not found");

        // Recusar lead
        lead.Decline();

        // Salvar mudanças
        await _leadRepository.UpdateAsync(lead);
        await _leadRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
