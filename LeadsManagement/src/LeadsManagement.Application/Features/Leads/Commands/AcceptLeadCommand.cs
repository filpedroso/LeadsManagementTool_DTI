namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;

/// <summary>
/// Command para aceitar um Lead
/// </summary>
public class AcceptLeadCommand : IRequest<Unit>
{
    public int LeadId { get; set; }
}
