using SimReport.Utils;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SimReport.Interfaces;

public interface IRepository<TModel, TViewModel>
{
    ValueTask<int> CreateAsync(TModel obj);
    ValueTask<int> UpdateAsync(int id, TModel editedObj);
    ValueTask<int> DeleteAsync(int id);
    ValueTask<IList<TViewModel>> GetAllAsync(PaginationParams @params);
    ValueTask<TViewModel> GetAsync(int id);
}
