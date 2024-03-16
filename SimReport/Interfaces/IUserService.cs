using SimReport.Utils;
using System.Threading.Tasks;
using SimReport.Entities.Users;
using System.Collections.Generic;

namespace SimReport.Interfaces;

public interface IUserService
{
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(long id);
    Task<User> GetByIdAsync(long id);
    Task<IEnumerable<User>> GetAllAsync();
}
