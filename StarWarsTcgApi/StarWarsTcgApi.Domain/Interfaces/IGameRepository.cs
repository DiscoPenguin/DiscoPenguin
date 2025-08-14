using StarWarsTcgApi.Domain.Models;

namespace StarWarsTcgApi.Domain.Interfaces
{
    public interface IGameRepository : IRepository<Game, int>
    {
        Task<IEnumerable<GameStatistics>> GetGameStatisticsAsync();
        Task<IEnumerable<Game>> GetGamesForPlayerAsync(int playerId);
    }
}