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
    Task<Response<bool>> DeleteAsync(long id);
    Task<Response<User>> GetAsync(long id);
    Task <Response<IEnumerable<User>>> GetAllAsync();
}
