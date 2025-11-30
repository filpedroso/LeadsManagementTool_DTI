namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Services;
using Mapster;

// A handler to process AcceptLeadCommand, apply discount if needed
// and send an email notification to "vendas@test.com"
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
        // Searches Lead
        var lead = await _leadRepository.GetByIdAsync(request.LeadId);
        if (lead == null)
            throw new InvalidOperationException($"Lead with id {request.LeadId} not found");

        var priceBeforeDiscount = lead.Price.Amount;
        lead.Accept();
        var priceAfterDiscount = lead.Price.Amount;
        var discountApplied = priceAfterDiscount < priceBeforeDiscount;

        // Saves changes
        await _leadRepository.UpdateAsync(lead);
        await _leadRepository.SaveChangesAsync();

        // Enviar notificação de email
        var emailAddress = "vendas@test.com";
        await _emailService.SendLeadAcceptedNotificationAsync(
            leadId: lead.Id,
            contactEmail: emailAddress,
            finalPrice: priceAfterDiscount,
            discountApplied: discountApplied);

        return Unit.Value;
    }
}
