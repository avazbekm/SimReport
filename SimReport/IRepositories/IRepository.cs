﻿using System;
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
    void Destroy(T entity);
    Task<T> GetAsync(Expression<Func<T, bool>> expression, string[] includes = null);
    IQueryable<T> GetAll(Expression<Func<T, bool>> expression = null, bool isNoTracked = true, string[] includes = null);

    Task SaveChanges();
}
