namespace LeadsManagement.Infrastructure.Data.Repositories;

using System.Linq.Expressions;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.Enums;
using LeadsManagement.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

public class LeadRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Lead> _dbSet;

    public LeadRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Lead>();
    }

    // Generic methods (from RepositoryBase)
    public async Task<Lead?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<Lead>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<Lead>> FindAsync(Expression<Func<Lead, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(Lead entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task UpdateAsync(Lead entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Lead entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    // Lead-specific methods
    public async Task<IEnumerable<Lead>> GetLeadsByStatusAsync(LeadStatus status)
    {
        return await _dbSet.Where(l => l.Status == status).ToListAsync();
    }

    public async Task<Lead?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(l => l.Contact.Email == email);
    }
}
