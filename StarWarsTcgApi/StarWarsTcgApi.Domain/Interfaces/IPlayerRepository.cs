using StarWarsTcgApi.Domain.Models;

namespace StarWarsTcgApi.Domain.Interfaces
{
    public interface IPlayerRepository : IRepository<Player, int>
    {
        Task<Player?> GetPlayerByGameAndUserAsync(int gameId, int userId);
        Task<IEnumerable<Player>> GetPlayersByGameIdAsync(int gameId);
    }
}