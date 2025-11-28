namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;

/// <summary>
/// Command para recusar um Lead
/// </summary>
public class DeclineLeadCommand : IRequest<Unit>
{
    public int LeadId { get; set; }
}
