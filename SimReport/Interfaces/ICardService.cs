using SimReport.Entities.Cards;
using SimReport.Services.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimReport.Interfaces;

public interface ICardService
{
    Task<Response<Card>> AddAsync(Card card);
    Task<Response<Card>> UpdateAsync(Card card);
    Task<Response<bool>> DeleteAsync(long id);
    Task<Response<Card>> GetAsync(long id);
    Task<Response<IEnumerable<Card>>> GetAllAsync();
}
