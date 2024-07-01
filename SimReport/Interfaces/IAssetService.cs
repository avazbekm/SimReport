using System.Threading.Tasks;
using SimReport.Entities.Assets;

namespace SimReport.Interfaces;

public interface IAssetService
{
    Task<Asset> UploadAsync();
}
