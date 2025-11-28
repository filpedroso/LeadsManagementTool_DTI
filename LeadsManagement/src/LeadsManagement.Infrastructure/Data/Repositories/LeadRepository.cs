namespace LeadsManagement.Infrastructure.Data.Repositories;

using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.Enums;
using LeadsManagement.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

// Repositório específico para entidade Lead
// Implementa operações específicas de Lead

public class LeadRepository : RepositoryBase<Lead>
{
    public LeadRepository() : base(null!) { }
    public LeadRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Busca todos os leads com um status específico
    /// </summary>
    public virtual async Task<IEnumerable<Lead>> GetLeadsByStatusAsync(LeadStatus status)
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
