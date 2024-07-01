using System.Threading.Tasks;
using SimReport.Entities.Users;
using SimReport.Services.Helpers;
using System.Collections.Generic;

namespace SimReport.Interfaces;

public interface IUserService
{
    Task<Response<User>> AddAsync(User user);
    Task<Response<User>> UpdateAsync(User user);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<User>> GetAsync(int id);
    Task<Response<User>> GetAsync(string phone);
    Task<Response<IEnumerable<User>>> GetAllAsync();
}
