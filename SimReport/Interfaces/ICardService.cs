using System.Threading.Tasks;
using SimReport.Entities.Cards;
using SimReport.Services.Helpers;
using System.Collections.Generic;

namespace SimReport.Interfaces;

public interface ICardService
{
    Task<Response<Card>> AddAsync(Card card);
    Task<Response<Card>> UpdateAsync(Card card);
    Task<Response<Card>> SellAsync(Card card);
    Task<Response<bool>> DeleteAsync(Card card);
    Task<Response<bool>> DeleteAsync(int id,string comment);
    Task<Response<bool>> DeleteAsync(int id, string first, string last);
    Task<Response<Card>> TransferAsync(Card card);
    Task<Response<bool>> ReturnAsync(long seriaNum, int id);
    Task<Response<Card>> GetAsync(int id);
    Task<Response<IEnumerable<Card>>> GetAllAsync();
    Task<Response<IEnumerable<Card>>> GetAllAsync(int companyId);
    Task<Response<IEnumerable<Card>>> GetAllAsync(int companyId, string first, string last);
    Task<Response<IEnumerable<Card>>> GetAllAsync(string phone);
}
