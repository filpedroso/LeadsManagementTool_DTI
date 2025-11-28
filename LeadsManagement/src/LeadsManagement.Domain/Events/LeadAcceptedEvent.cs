namespace LeadsManagement.Domain.Events;

/// <summary>
/// Evento disparado quando um lead é aceito
/// </summary>
public record LeadAcceptedEvent : DomainEvent
{
    public int LeadId { get; init; }
    public decimal FinalPrice { get; init; }
    public bool DiscountApplied { get; init; }

    public LeadAcceptedEvent(int leadId, decimal finalPrice, bool discountApplied, DateTime occurredAt)
    {
        LeadId = leadId;
        FinalPrice = finalPrice;
        DiscountApplied = discountApplied;
        OccurredAt = occurredAt;
    }
}
