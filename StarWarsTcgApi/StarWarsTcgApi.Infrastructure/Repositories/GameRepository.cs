using Dapper;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Domain.Models;
using StarWarsTcgApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Infrastructure.Repositories
{
    public class GameRepository : GenericRepository<Game, int>, IGameRepository
    {
        public GameRepository(MySqlDataAccess dataAccess) : base(dataAccess, "game") { }

        public async Task<IEnumerable<GameStatistics>> GetGameStatisticsAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM vwGameStatistics";
            return await connection.QueryAsync<GameStatistics>(sql);
        }

        public async Task<IEnumerable<Game>> GetGamesForPlayerAsync(int playerId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE LightPlayerId = @playerId OR DarkPlayerId = @playerId";
            return await connection.QueryAsync<Game>(sql, new { playerId = playerId });
        }

        // Example of overriding AddAsync to handle Guid generation
        public override async Task<int> AddAsync(Game game)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = @"
                INSERT INTO Games (LightPlayerId, DarkPlayerId, CurrentTurnUserId, CurrentPhaseId, GameStatusId, WinnerPlayerId)
                VALUES (@LightPlayerId, @DarkPlayerId, @CurrentTurnUserId, @CurrentPhaseId, @GameStatusId, @WinnerPlayerId)";
            return await connection.ExecuteAsync(sql, game);
        }

        public override async Task UpdateAsync(Game game)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = @"
                UPDATE Games SET
                    LightPlayerId = @LightPlayerId,
                    DarkPlayerId = @DarkPlayerId,
                    CurrentTurnUserId = @CurrentTurnUserId,
                    CurrentPhaseId = @CurrentPhaseId,
                    GameStatusId = @GameStatusId,
                    WinnerPlayerId = @WinnerPlayerId,
                    LastUpdated = @LastUpdated
                WHERE Id = @Id";
            await connection.ExecuteAsync(sql, game);
        }
    }
}