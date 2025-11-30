#nullable disable

namespace LeadsManagement.Infrastructure.Data.Contexts;

using Microsoft.EntityFrameworkCore;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Infrastructure.Data.Configurations;

// Configures the interaction with the DB
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() : base(null!) { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Lead> Leads { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new LeadConfiguration());
    }
}
