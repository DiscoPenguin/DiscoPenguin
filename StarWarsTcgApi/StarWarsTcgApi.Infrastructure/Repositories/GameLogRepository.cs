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
    public class GameLogRepository : GenericRepository<GameLog, int>, IGameLogRepository
    {
        public GameLogRepository(MySqlDataAccess dataAccess) : base(dataAccess, "game_log") { }

        public async Task<IEnumerable<Actions>> GetAllActionTypesAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM actions";
            return await connection.QueryAsync<Actions>(sql);
        }

        public async Task<IEnumerable<GamePhase>> GetAllGamePhasesAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM game_phase";
            return await connection.QueryAsync<GamePhase>(sql);
        }

        public async Task<IEnumerable<GameStatus>> GetAllGameStatusesAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM game_status";
            return await connection.QueryAsync<GameStatus>(sql);
        }

        public async Task<IEnumerable<GameLog>> GetGameLogsByGameIdAsync(int gameId, int? skip = null, int? take = null)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM game_log WHERE gameId = @gameId";
            return await connection.QueryAsync<GameLog>(sql, new { GameId = gameId });
        }
    }
}