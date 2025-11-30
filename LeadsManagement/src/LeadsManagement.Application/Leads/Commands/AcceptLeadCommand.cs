namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;

// Command to accept a Lead
public class AcceptLeadCommand : IRequest<Unit>
{
    public int LeadId { get; set; }
}
