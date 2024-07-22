using System;
using System.Linq;
using SimReport.Contants;
using SimReport.Entities;
using SimReport.Interfaces;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SimReport.Repositories;

public class Repository<T> : IRepository<T> where T : Auditable
{ 
    private readonly DbSet<T> dbSet;
    private readonly AppDbContext appDbContext;

    public Repository()
    {
        this.appDbContext = new AppDbContext();
        dbSet = appDbContext.Set<T>();
    }

    public async Task CreateAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        appDbContext.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        appDbContext.Entry(entity).State = EntityState.Modified;
    }

    public void Destroy(T entity)
    {
        dbSet.Remove(entity);
    }

    public IQueryable<T> GetAll(Expression<Func<T, bool>> expression = null, bool isNoTracked = true, string[] includes = null)
    {
        IQueryable<T> query = expression is null ? dbSet.AsQueryable() : dbSet.Where(expression).AsQueryable();
        query = isNoTracked ? query.AsNoTracking() : query;

        if (includes is not null)
            foreach (var item in includes)
                query = query.Include(item);
        return query.Where(a=>!a.IsDeleted);
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> expression, string[] includes = null)
    {
        IQueryable<T> query = dbSet.AsQueryable().Where(a=>!a.IsDeleted);
        if (includes is not null)
            foreach (var include in includes)
                query = query.Include(include);
        var entity = await query.FirstOrDefaultAsync(expression);
        return entity; 
    }

    public async Task SaveChanges()
    {
        await appDbContext.SaveChangesAsync();
    }

    public IQueryable<T> GetDeleteAll(Expression<Func<T, bool>> expression = null, bool isNoTracked = true, string[] includes = null)
    {
        IQueryable<T> query = expression is null ? dbSet.AsQueryable() : dbSet.Where(expression).AsQueryable();
        query = isNoTracked ? query.AsNoTracking() : query;

        if (includes is not null)
            foreach (var item in includes)
                query = query.Include(item);
        return query.Where(a => a.IsDeleted);
    }

}