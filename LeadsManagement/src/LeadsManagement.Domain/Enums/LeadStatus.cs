namespace LeadsManagement.Domain.Enums;

/// <summary>
/// Enum que representa os possíveis status de um Lead
/// </summary>
public enum LeadStatus
{
    /// <summary>
    /// Lead convidado (novo)
    /// </summary>
    Invited = 0,

    /// <summary>
    /// Lead aceito
    /// </summary>
    Accepted = 1,

    /// <summary>
    /// Lead recusado
    /// </summary>
    Declined = 2
}
