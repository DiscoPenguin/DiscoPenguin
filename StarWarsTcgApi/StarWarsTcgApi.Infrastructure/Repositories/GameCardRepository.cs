using Dapper;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Domain.Models;
using StarWarsTcgApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Infrastructure.Repositories
{
    public class GameCardRepository : GenericRepository<GameCard, int>, IGameCardRepository
    {
        public GameCardRepository(MySqlDataAccess dataAccess) : base(dataAccess, "game_cards") { }

        public async Task<IEnumerable<GameZone>> GetAllCardZonesAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM game_zone";
            return await connection.QueryAsync<GameZone>(sql);
        }

        public Task<IEnumerable<GameCard>> GetGameCardsInPlayByGameIdAsync(int gameId)
        {
            //NOTE: GetGameCardsInPlayByGameIdAsync seems kind of useless..?
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GameCard>> GetGameCardsInZoneAsync(int gameId, int zoneId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE GameId = @gameId AND CurrentZoneId = @zoneId";
            return await connection.QueryAsync<GameCard>(sql, new { gameId = gameId, zoneId = zoneId});
        }

        public async Task<IEnumerable<GameCard>> GetGameCardsOwnedByPlayerInZoneAsync(int gameId, int ownerPlayerId, int zoneId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE GameId = @gameId AND CurrentZoneId = @zoneId and OwnerPlayerId = @ownerPlayerId";
            return await connection.QueryAsync<GameCard>(sql, new { gameId = gameId, zoneId = zoneId, ownerPlayerId = ownerPlayerId});
        }

        public async Task<IEnumerable<GameCard>> GetGameCardsControlledByPlayerInZoneAsync(int gameId, int controllerPlayerId, int zoneId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE GameId = @gameId AND CurrentZoneId = @zoneId and ControllerPlayerId = @controllerPlayerId";
            return await connection.QueryAsync<GameCard>(sql, new { gameId = gameId, zoneId = zoneId, controllerPlayerId = controllerPlayerId});
        }

        public async Task<GameCard?> GetLocationCardInPlayAreaAsync(int gameId, int playAreaZoneId)
        {
            // game_zone.CanHaveLocation, "select * from wotc where Type = 'Location'"
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT gc.* FROM {_tableName} gc INNER JOIN game_zone gz ON (gc.CurrentZoneId = gz.Id AND gz.CanHasLocation = true) INNER JOIN wotc ON (gc.CardId = wotc.Id) WHERE GameId = @gameId AND CurrentZoneId = @zoneId and wotc.Type = 'Location'";
            return await connection.QuerySingleOrDefaultAsync<GameCard>(sql, new { gameId = gameId, zoneId = playAreaZoneId });
        }
    }
}