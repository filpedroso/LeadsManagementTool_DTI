namespace LeadsManagement.Domain.Services;

// Email service for when a Lead is succesfully accepted
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, string? htmlBody = null);
    Task SendLeadAcceptedNotificationAsync(int leadId, string contactEmail, decimal finalPrice, bool discountApplied);
}
