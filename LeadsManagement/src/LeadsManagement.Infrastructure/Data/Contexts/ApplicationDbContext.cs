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
