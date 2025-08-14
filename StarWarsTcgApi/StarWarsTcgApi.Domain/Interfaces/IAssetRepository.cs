using StarWarsTcgApi.Domain.Models;

namespace StarWarsTcgApi.Domain.Interfaces
{
    public interface IAssetRepository : IRepository<Asset, int>
    {
        Task<IEnumerable<Asset>> GetAllAssetsAsync();
        Task<Asset> GetAssetByIdAsync(int assetId);
    }
}