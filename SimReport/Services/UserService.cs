using System;
using System.Linq;
using SimReport.Interfaces;
using System.Threading.Tasks;
using SimReport.Entities.Users;
using System.Collections.Generic;
using SimReport.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SimReport.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> userRepository;

    public UserService(IRepository<User> userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<User> CreateAsync(User user)
    {
        var users = await userRepository.GetAll().ToListAsync();
        var existUser = users.FirstOrDefault(x => x.Phone.Equals(user.Phone));
        if (existUser is not null)
            throw new AlreadyExistException($"This phone is already exist {user.Phone}");

        await userRepository.CreateAsync(user);
        await userRepository.SaveAsync();
        
        return user;
    }

    public Task<bool> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<User> UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }
}
