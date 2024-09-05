using SimReport.Entities.Block;
using SimReport.Entities.Cards;
using SimReport.Services.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimReport.Interfaces;

public interface IBlockService
{
    Task<Response<BlockDate>> AddAsync(BlockDate date);
    Task<Response<BlockDate>> UpdateAsync();
    Task<Response<bool>> DeleteAsync(BlockDate date);
    Task<Response<IEnumerable<BlockDate>>> GetAllAsync();

}
