namespace LeadsManagement.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Domain.Enums;

// Configures the mapping between object and DB
// for the Entity Framework
public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("Leads");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Contact (Value Object)
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

        // Price (Value Object)
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

        builder.HasIndex(x => x.Status).HasDatabaseName("IX_Lead_Status");
        builder.HasIndex(x => x.DateCreated).HasDatabaseName("IX_Lead_DateCreated");
    }
}
