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
    protected readonly ApplicationDbContext? Context;
    protected readonly DbSet<TEntity>? DbSet;

    public RepositoryBase() { }

    public RepositoryBase(ApplicationDbContext context)
    {
        if (context != null)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return Context == null ? null : await DbSet!.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return Context == null ? new List<TEntity>() : await DbSet!.ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Context == null ? new List<TEntity>() : await DbSet!.Where(predicate).ToListAsync();
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        if (Context != null)
            await DbSet!.AddAsync(entity);
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        if (Context != null)
            DbSet!.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(TEntity entity)
    {
        if (Context != null)
            DbSet!.Remove(entity);
        await Task.CompletedTask;
    }

    public virtual async Task SaveChangesAsync()
    {
        if (Context != null)
            await Context.SaveChangesAsync();
    }
}
