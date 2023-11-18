using SimReport.Utils;
using SimReport.Interfaces;
using System.Threading.Tasks;
using SimReport.Entities.Users;
using System.Collections.Generic;

namespace SimReport.Repositories.Users;

public class UserRepository : IUserRepository
{
    public ValueTask<int> CreateAsync(User obj)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<int> DeleteAsync(int id)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<IList<User>> GetAllAsync(PaginationParams @params)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<User> GetAsync(int id)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<int> UpdateAsync(int id, User editedObj)
    {
        throw new System.NotImplementedException();
    }
}
