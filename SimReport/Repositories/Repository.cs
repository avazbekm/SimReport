using System;
using System.Linq;
using SimReport.Contants;
using SimReport.Entities;
using SimReport.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        appDbContext.Entry(entity).State = EntityState.Modified;
    }

    public void Destroy(T entity)
    {
        dbSet.Remove(entity);
    }

    public IQueryable<T> GetAll()
        => dbSet.AsNoTracking().AsQueryable();

    public async Task SaveChanges()
    {
        await appDbContext.SaveChangesAsync();
    }
}