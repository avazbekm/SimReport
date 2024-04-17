using SimReport.Entities.Companies;
using SimReport.Services.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimReport.Interfaces;

public interface ICompanyService
{
    Task<Response<Company>> AddAsync(Company company);
    Task<Response<Company>> UpdateAsync(Company company);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<Company>> GetAsync(int id);
    Task<Response<Company>> GetAsync(string name);
    Task<Response<IEnumerable<Company>>> GetAllAsync();
}
