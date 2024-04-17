using SimReport.Entities.Assets;
using System.Threading.Tasks;

namespace SimReport.Interfaces;

public interface IAssetService
{
    Task<Asset> UploadAsync();
}
