namespace LeadsManagement.Domain.Events;

/// <summary>
/// Evento disparado quando um lead é recusado
/// </summary>
public record LeadDeclinedEvent : DomainEvent
{
    public int LeadId { get; init; }

    public LeadDeclinedEvent(int leadId, DateTime occurredAt)
    {
        LeadId = leadId;
        OccurredAt = occurredAt;
    }
}
