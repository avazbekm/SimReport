using System;
using SimReport.Interfaces;
using System.Threading.Tasks;
using SimReport.Entities.Users;
using SimReport.Services.Helpers;
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

    public async Task<Response<User>> AddAsync(User user)
    {
        try
        {
            var existUser = await this.userRepository.GetAll().FirstOrDefaultAsync(u => u.Phone.Equals(user.Phone));
            if (existUser is not null)
                throw new AlreadyExistException("Bunday nomer mavjud.");

            await this.userRepository.CreateAsync(user);
            await this.userRepository.SaveChanges();

            return new Response<User>
            {
                StatusCode = 200,
                Message = "Saqlandi.",
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
                Message = "O'chirildi.",
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

    public async Task<Response<User>> GetAsync(long id)
    {
        try
        {
            var existUser = await this.userRepository.GetAll().FirstOrDefaultAsync(u => u.Id.Equals(id));
            if (existUser is null)
                throw new NotFoundException("Buday telefon foydalanuchi mavjud emas.");

            return new Response<User>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = existUser
            };
        }
        catch (Exception ex)
        {
            return new Response<User>
            {
                StatusCode = 403,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<Response<User>> UpdateAsync(User user)
    {
        try
        {
            var existUser = await this.userRepository.GetAll().FirstOrDefaultAsync(u => u.Phone.Equals(user.Phone));
            if (existUser != null)
                throw new AlreadyExistException("Bunday nomer foydalanuvchisi mavjud emas!");

            existUser.FirstName=user.FirstName;
            existUser.LastName=user.LastName;
            existUser.Phone=user.Phone;

            this.userRepository.Update(user);
            await this.userRepository.SaveChanges();

            return new Response<User>
            {
                StatusCode = 200,
                Message = "Malumotlar o'zgardi.",
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
}
