namespace LeadsManagement.Domain.Events;

/// <summary>
/// Classe base para eventos de domínio
/// </summary>
public abstract record DomainEvent
{
    public DateTime OccurredAt { get; init; }
    public Guid Id { get; init; } = Guid.NewGuid();
}
