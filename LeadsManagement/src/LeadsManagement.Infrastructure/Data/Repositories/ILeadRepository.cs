namespace LeadsManagement.Infrastructure.Data.Repositories;

using System.Linq.Expressions;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.Enums;

public interface ILeadRepository
{
    Task<Lead?> GetByIdAsync(int id);
    Task<IEnumerable<Lead>> GetAllAsync();
    Task<IEnumerable<Lead>> FindAsync(Expression<Func<Lead, bool>> predicate);
    Task AddAsync(Lead entity);
    Task UpdateAsync(Lead entity);
    Task DeleteAsync(Lead entity);
    Task SaveChangesAsync();
    Task<IEnumerable<Lead>> GetLeadsByStatusAsync(LeadStatus status);
    Task<Lead?> GetByEmailAsync(string email);
}
