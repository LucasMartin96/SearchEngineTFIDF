using Microsoft.EntityFrameworkCore;
using SearchEngine.Core.Entities.Base;
using SearchEngine.Core.Interfaces.Repositories;
using SearchEngine.Persistence.Context;

namespace SearchEngine.Persistence.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly SearchEngineContext Context;
    protected readonly DbSet<T> Entities;

    public BaseRepository(SearchEngineContext context)
    {
        Context = context;
        Entities = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await Entities.FindAsync(id);
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await Entities.ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await Entities.AddAsync(entity);
        return entity;
    }

    public virtual Task UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Entities.Update(entity);
        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        T? entity = await GetByIdAsync(id);
        if (entity != null)
            Entities.Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }

    public virtual async Task<int> GetTotalCountAsync()
    {
        return await Entities.CountAsync();
    }
}