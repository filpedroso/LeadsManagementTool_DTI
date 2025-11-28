// ===================================================================
// FILE: src/LeadsManagement.Infrastructure/Data/Contexts/ApplicationDbContext.cs
// ===================================================================

namespace LeadsManagement.Infrastructure.Data.Contexts;

using Microsoft.EntityFrameworkCore;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Infrastructure.Data.Configurations;

/// <summary>
/// DbContext principal da aplicação
/// Configura todas as entidades e seus mapeamentos
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Lead> Leads { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações
        modelBuilder.ApplyConfiguration(new LeadConfiguration());
    }
}

// ===================================================================
// FILE: src/LeadsManagement.Infrastructure/Data/Configurations/LeadConfiguration.cs
// ===================================================================

namespace LeadsManagement.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Domain.Enums;

/// <summary>
/// Configuração Fluent API para a entidade Lead
/// Define mapeamento com banco de dados
/// </summary>
public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        // Tabela
        builder.ToTable("Leads");

        // Chave primária
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Contact (Value Object) - Componente de valor
        builder.OwnsOne(x => x.Contact, contact =>
        {
            contact.Property(c => c.FirstName)
                .HasColumnName("ContactFirstName")
                .HasMaxLength(100)
                .IsRequired();

            contact.Property(c => c.LastName)
                .HasColumnName("ContactLastName")
                .HasMaxLength(100);

            contact.Property(c => c.PhoneNumber)
                .HasColumnName("ContactPhoneNumber")
                .HasMaxLength(20);

            contact.Property(c => c.Email)
                .HasColumnName("ContactEmail")
                .HasMaxLength(255);
        });

        // Price (Value Object) - Componente de valor
        builder.OwnsOne(x => x.Price, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("Price")
                .HasPrecision(18, 2)
                .IsRequired();

            price.Property(p => p.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Propriedades simples
        builder.Property(x => x.DateCreated)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.Suburb)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Category)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.ToString(),
                v => (LeadStatus)Enum.Parse(typeof(LeadStatus), v))
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue(LeadStatus.Invited);

        // Índices
        builder.HasIndex(x => x.Status).HasName("IX_Lead_Status");
        builder.HasIndex(x => x.DateCreated).HasName("IX_Lead_DateCreated");
    }
}

// ===================================================================
// FILE: src/LeadsManagement.Infrastructure/Data/Repositories/IRepository.cs
// ===================================================================

namespace LeadsManagement.Infrastructure.Data.Repositories;

using System.Linq.Expressions;

/// <summary>
/// Interface genérica para repositório
/// Define contrato para operações CRUD
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task SaveChangesAsync();
}

// ===================================================================
// FILE: src/LeadsManagement.Infrastructure/Data/Repositories/RepositoryBase.cs
// ===================================================================

namespace LeadsManagement.Infrastructure.Data.Repositories;

using System.Linq.Expressions;
using LeadsManagement.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementação base do repositório genérico
/// Fornece operações CRUD comuns
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
public class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public RepositoryBase(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(TEntity entity)
    {
        DbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public virtual async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }
}

// ===================================================================
// FILE: src/LeadsManagement.Infrastructure/Data/Repositories/LeadRepository.cs
// ===================================================================

namespace LeadsManagement.Infrastructure.Data.Repositories;

using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.Enums;
using LeadsManagement.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repositório específico para entidade Lead
/// Implementa operações específicas de Lead
/// </summary>
public class LeadRepository : RepositoryBase<Lead>
{
    public LeadRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Busca todos os leads com um status específico
    /// </summary>
    public async Task<IEnumerable<Lead>> GetLeadsByStatusAsync(LeadStatus status)
    {
        return await DbSet
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.DateCreated)
            .ToListAsync();
    }

    /// <summary>
    /// Busca leads por categoria
    /// </summary>
    public async Task<IEnumerable<Lead>> GetLeadsByCategoryAsync(string category)
    {
        return await DbSet
            .Where(x => x.Category == category)
            .OrderByDescending(x => x.DateCreated)
            .ToListAsync();
    }

    /// <summary>
    /// Busca leads dentro de um range de preço
    /// </summary>
    public async Task<IEnumerable<Lead>> GetLeadsInPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await DbSet
            .Where(x => x.Price.Amount >= minPrice && x.Price.Amount <= maxPrice)
            .OrderByDescending(x => x.DateCreated)
            .ToListAsync();
    }
}

// ===================================================================
// FILE: src/LeadsManagement.Infrastructure/Services/IEmailService.cs
// ===================================================================

namespace LeadsManagement.Infrastructure.Services;

/// <summary>
/// Contrato para serviço de email
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, string? htmlBody = null);
    Task SendLeadAcceptedNotificationAsync(int leadId, string contactEmail, decimal finalPrice, bool discountApplied);
}

// ===================================================================
// FILE: src/LeadsManagement.Infrastructure/Services/EmailService.cs
// ===================================================================

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
