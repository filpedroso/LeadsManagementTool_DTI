namespace LeadsManagement.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using System.Text;

/// <summary>
/// Implementação do serviço de email
/// Para desenvolvimento: simula envio salvando em arquivo
/// Para produção: implementar com SMTP real
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, string? htmlBody = null)
    {
        try
        {
            // Para desenvolvimento, salva em arquivo ao invés de enviar de verdade
            var emailContent = new StringBuilder();
            emailContent.AppendLine("=== EMAIL SIMULADO ===");
            emailContent.AppendLine($"Para: {to}");
            emailContent.AppendLine($"Assunto: {subject}");
            emailContent.AppendLine($"Data: {DateTime.UtcNow:O}");
            emailContent.AppendLine("---");
            emailContent.AppendLine(body);
            if (!string.IsNullOrEmpty(htmlBody))
            {
                emailContent.AppendLine("---HTML---");
                emailContent.AppendLine(htmlBody);
            }
            emailContent.AppendLine("======================");

            var filePath = Path.Combine(
                AppContext.BaseDirectory,
                "emails",
                $"email_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid()}.txt");

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            await File.WriteAllTextAsync(filePath, emailContent.ToString());

            _logger.LogInformation($"Email simulado salvo em: {filePath}");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao enviar email para {to}");
            throw;
        }
    }

    public async Task SendLeadAcceptedNotificationAsync(int leadId, string contactEmail, decimal finalPrice, bool discountApplied)
    {
        var subject = "Lead Accepted - Sales Notification";
        var body = $@"
Dear Sales Team,

A lead has been accepted.

Lead ID: {leadId}
Contact Email: {contactEmail}
Final Price: ${finalPrice:F2}
Discount Applied: {(discountApplied ? "Yes (10%)" : "No")}

Please follow up accordingly.

Best regards,
Leads Management System
";

        var htmlBody = $@"
<html>
<body>
    <h2>Lead Accepted - Sales Notification</h2>
    <p>Dear Sales Team,</p>
    <p>A lead has been accepted.</p>
    <ul>
        <li><strong>Lead ID:</strong> {leadId}</li>
        <li><strong>Contact Email:</strong> {contactEmail}</li>
        <li><strong>Final Price:</strong> ${finalPrice:F2}</li>
        <li><strong>Discount Applied:</strong> {(discountApplied ? "Yes (10%)" : "No")}</li>
    </ul>
    <p>Please follow up accordingly.</p>
    <p>Best regards,<br/>Leads Management System</p>
</body>
</html>
";

        await SendEmailAsync("vendas@test.com", subject, body, htmlBody);
    }
}
