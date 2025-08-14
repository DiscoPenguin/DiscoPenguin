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
    public class PlayerRepository : GenericRepository<Player, int>, IPlayerRepository
    {
        public PlayerRepository(MySqlDataAccess dataAccess) : base(dataAccess, "Player") { }

        public async Task<Player?> GetPlayerByGameAndUserAsync(int gameId, int userId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE UserId = @userId AND GameId = @gameId";
            return await connection.QuerySingleOrDefaultAsync<Player>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<Player>> GetPlayersByGameIdAsync(int gameId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE GameId = @gameId";
            return await connection.QueryAsync<Player>(sql, new { GameId = gameId });
        }
    }
}