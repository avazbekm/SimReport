using System;
using System.Linq;
using SimReport.Entities;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SimReport.Interfaces;

public interface IRepository<T> where T : Auditable
{
    Task CreateAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<T> GetByIdAsync(long id);
    IQueryable<T> GetAll();
    Task SaveAsync();
}
