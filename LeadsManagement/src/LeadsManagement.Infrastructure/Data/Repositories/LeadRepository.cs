namespace LeadsManagement.Infrastructure.Data.Repositories;

using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.Enums;
using LeadsManagement.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

// // Search leads by status on DB
public class LeadRepository : RepositoryBase<Lead>
{
    public LeadRepository() : base(null!) { }
    public LeadRepository(ApplicationDbContext context) : base(context) { }

    public virtual async Task<IEnumerable<Lead>> GetLeadsByStatusAsync(LeadStatus status)
    {
        return await DbSet
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.DateCreated)
            .ToListAsync();
    }
}
