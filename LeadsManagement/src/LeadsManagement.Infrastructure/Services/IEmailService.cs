namespace LeadsManagement.Infrastructure.Services;

/// <summary>
/// Contrato para serviço de email
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, string? htmlBody = null);
    Task SendLeadAcceptedNotificationAsync(int leadId, string contactEmail, decimal finalPrice, bool discountApplied);
}
