using System;
using System.Linq;
using SimReport.Interfaces;
using System.Threading.Tasks;
using SimReport.Services.Helpers;
using System.Collections.Generic;
using SimReport.Entities.Companies;
using SimReport.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SimReport.Services;

public class CompanyService : ICompanyService
{
    private readonly IRepository<Company> companyRepository;
    public CompanyService(IRepository<Company> companyRepository)
    {

        this.companyRepository = companyRepository;
    }
    public async Task<Response<Company>> AddAsync(Company company)
    {
        try
        {
            var existCompany = this.companyRepository.GetAll().FirstOrDefault(c => c.Name.Equals(company.Name));
            if (existCompany is not null)
                throw new AlreadyExistException("Bu nom bilan kompaniya mavjud.");

            await this.companyRepository.CreateAsync(company);
            await this.companyRepository.SaveChanges();

            return new Response<Company>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = company
            };
        }
        catch (Exception ex)
        {
            return new Response<Company>
            {
                StatusCode = 403,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<Response<bool>> DeleteAsync(long id)
    {
        var existCompany = this.companyRepository.GetAll().FirstOrDefault(u => u.Id.Equals(id));
        if (existCompany is null)
            throw new NotFoundException("Bu nom bilan kompaniya mavjud emas.");
        else
        {
            this.companyRepository.Delete(existCompany);
            await this.companyRepository.SaveChanges();
            return new Response<bool>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = true
            };
        }
    }

    public async Task<Response<IEnumerable<Company>>> GetAllAsync()
    {
        var companies = await this.companyRepository.GetAll().ToListAsync();
        return new Response<IEnumerable<Company>>
        {
            StatusCode = 200,
            Message = "Ok",
            Data = companies
        };
    }

    public async Task<Response<Company>> GetAsync(long id)
    {
        try
        {
            var existCompany = await this.companyRepository.GetAll().FirstOrDefaultAsync(u => u.Id.Equals(id));
            if (existCompany is null)
                throw new NotFoundException("Buday nom bilan kompaniya mavjud emas.");

            return new Response<Company>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = existCompany
            };
        }
        catch (Exception ex)
        {
            return new Response<Company>
            {
                StatusCode = 403,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<Response<Company>> UpdateAsync(Company company)
    {
        try
        {
            var existCompany = await this.companyRepository.GetAll().FirstOrDefaultAsync(u => u.Name.Equals(company.Name));
            if (existCompany != null)
                throw new AlreadyExistException("Bunday nomer foydalanuvchisi mavjud emas!");

            existCompany.Name = company.Name;

            this.companyRepository.Update(company);
            await this.companyRepository.SaveChanges();

            return new Response<Company>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = company
            };
        }
        catch (Exception ex)
        {
            return new Response<Company>
            {
                StatusCode = 403,
                Message = ex.Message,
                Data = null
            };
        }
    }
}
