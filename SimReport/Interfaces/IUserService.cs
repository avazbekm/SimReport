using System.Threading.Tasks;
using SimReport.Entities.Users;
using System.Collections.Generic;
using SimReport.Entities;
using System.Linq.Expressions;
using System.Linq;
using System;
using SimReport.Utils;
using SimReport.Services.Helpers;

namespace SimReport.Interfaces;

public interface IUserService
{
    Task<Response<User>> AddAsync(User user);
    Task<Response<User>> UpdateAsync(User user);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<User>> GetAsync(int id);
    Task<Response<User>> GetAsync(string phone);
    Task <Response<IEnumerable<User>>> GetAllAsync();
}
