using StarWarsTcgApi.Domain.Models;

namespace StarWarsTcgApi.Domain.Interfaces
{
    public interface IGameCardRepository : IRepository<GameCard, int>
    {
        Task<IEnumerable<GameCard>> GetGameCardsInZoneAsync(int gameId, int zoneId);
        Task<IEnumerable<GameCard>> GetGameCardsOwnedByPlayerInZoneAsync(int gameId, int ownerPlayerId, int zoneId);
        Task<IEnumerable<GameCard>> GetGameCardsInPlayByGameIdAsync(int gameId);
        Task<GameCard?> GetLocationCardInPlayAreaAsync(int gameId, int playAreaZoneId);

        #region static lookup data
        Task<IEnumerable<GameZone>> GetAllCardZonesAsync();
        #endregion
    }
}