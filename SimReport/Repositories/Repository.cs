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
    private readonly AppDbContext appDbContext;
    private readonly DbSet<T> dbSet;
    public Repository(IUserService? userService)
    {
        this.appDbContext = new AppDbContext();
        this.dbSet = appDbContext.Set<T>();
    }

    public async Task CreateAsync(T entity)
    {
        await this.dbSet.AddAsync(entity);
    }

    public void Delete(T entity)
    {
        this.dbSet.Remove(entity);
    }

    public IQueryable<T> GetAll()
        => dbSet.AsNoTracking().AsQueryable();

    public Task<T> GetByIdAsync(long id)
        => dbSet.FirstOrDefaultAsync(t => t.Id.Equals(id));

    public void Update(T entity)
    {
        appDbContext.Entry(entity).State = EntityState.Modified;
    }

    public async Task SaveAsync()
    {
        appDbContext.SaveChanges();
    }
}
