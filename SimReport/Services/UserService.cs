using Microsoft.EntityFrameworkCore;
using SimReport.Entities.Users;
using SimReport.Interfaces;
using SimReport.Services.Exceptions;
using SimReport.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimReport.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> userRepository;

    public UserService(IRepository<User> userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<Response<User>> AddAsync(User user)
    {
        try
        {
            var existUser = await this.userRepository.GetAll().FirstOrDefaultAsync(u => u.Phone.Equals(user.Phone));
            if (existUser != null)
                throw new AlreadyExistException("This phone is already exist");

            await this.userRepository.CreateAsync(user);
            await this.userRepository.SaveChanges();

            return new Response<User>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = user
            };
        }
        catch (Exception ex)
        {
            return new Response<User>
            {
                StatusCode = 403,
                Message = ex.Message,
                Data = user
            };
        }
    }

    public async Task<Response<bool>> DeleteAsync(long id)
    {
        var existUser = await this.userRepository.GetAll().FirstOrDefaultAsync(u => u.Id.Equals(id));
        if (existUser is null)
            throw new NotFoundException("Bu nomerdagi foydalanuchi mavjud emas.");
        else
        {

            this.userRepository.Delete(existUser);
            await this.userRepository.SaveChanges();
            return new Response<bool>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = true
            };
        }
    }

    public async Task<Response<IEnumerable<User>>> GetAllAsync()
    {
        var users = await this.userRepository.GetAll().ToListAsync();
        return new Response<IEnumerable<User>>
        {
            StatusCode = 200,
            Message = "Ok",
            Data = users
        };
    }

    public Task<Response<User>> GetAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<User>> UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }
}
