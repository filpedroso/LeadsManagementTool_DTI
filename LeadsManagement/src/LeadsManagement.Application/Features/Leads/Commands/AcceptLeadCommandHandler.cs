namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Infrastructure.Services;
using Mapster;

/// <summary>
/// Handler que processa AcceptLeadCommand
/// Aceita um lead, aplica desconto se aplicável
/// E envia notificação por email
/// </summary>
public class AcceptLeadCommandHandler : IRequestHandler<AcceptLeadCommand, Unit>
{
    private readonly LeadRepository _leadRepository;
    private readonly IEmailService _emailService;

    public AcceptLeadCommandHandler(LeadRepository leadRepository, IEmailService emailService)
    {
        _leadRepository = leadRepository;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(AcceptLeadCommand request, CancellationToken cancellationToken)
    {
        // Buscar lead
        var lead = await _leadRepository.GetByIdAsync(request.LeadId);
        if (lead == null)
            throw new InvalidOperationException($"Lead with id {request.LeadId} not found");

        // Aceitar lead (aplica lógica de desconto)
        var priceBeforeDiscount = lead.Price.Amount;
        lead.Accept();
        var priceAfterDiscount = lead.Price.Amount;
        var discountApplied = priceAfterDiscount < priceBeforeDiscount;

        // Salvar mudanças
        await _leadRepository.UpdateAsync(lead);
        await _leadRepository.SaveChangesAsync();

        // Enviar notificação de email
        var emailAddress = lead.Contact.Email ?? "vendas@test.com";
        await _emailService.SendLeadAcceptedNotificationAsync(
            leadId: lead.Id,
            contactEmail: emailAddress,
            finalPrice: priceAfterDiscount,
            discountApplied: discountApplied);

        return Unit.Value;
    }
}
