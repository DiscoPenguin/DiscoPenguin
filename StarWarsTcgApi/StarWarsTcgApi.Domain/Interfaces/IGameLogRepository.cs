using StarWarsTcgApi.Domain.Models;

namespace StarWarsTcgApi.Domain.Interfaces
{
    public interface IGameLogRepository : IRepository<GameLog, int>
    {
        Task<IEnumerable<GameLog>> GetGameLogsByGameIdAsync(int gameId, int? skip = null, int? take = null);

        #region Static Data
        Task<IEnumerable<Actions>> GetAllActionTypesAsync();
        Task<IEnumerable<GamePhase>> GetAllGamePhasesAsync();
        Task<IEnumerable<GameStatus>> GetAllGameStatusesAsync();
        #endregion
    }
}