using Dapper;
using MySql.Data.MySqlClient;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Domain.Models;
using StarWarsTcgApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Infrastructure.Repositories
{
    public class DeckBuilderRepository : GenericRepository<DeckBuilder, int>, IDeckBuilderRepository
    {
        public DeckBuilderRepository(MySqlDataAccess dataAccess) : base(dataAccess, "DeckBuilder") { }

/*
        public async new Task<DeckBuilder> GetByIdAsync(int id)
        {
            return await _context.DeckBuilders
                .Include(db => db.DeckBuilderCards)
                .ThenInclude(dbc => dbc.Card) // Assuming Card is needed for Details
                .FirstOrDefaultAsync(db => db.Id == id);
        }
*/
        public async Task<IEnumerable<DeckBuilder>> GetCardsByDeckAsync(int deckId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE DeckId = @deckId";
            return await connection.QueryAsync<DeckBuilder>(sql, new { DeckId = deckId });
        }

        public async Task<IEnumerable<DeckBuilder>> GetDecksByCardAsyc(int cardId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE CardId = @cardId";
            return await connection.QueryAsync<DeckBuilder>(sql, new { CardId = cardId });
        }
        public async Task<int> GetDeckCardQuantity(int deckId, int cardId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT Quantity FROM {_tableName} WHERE DeckId = @deckId AND CardId = @cardId";
            int? quantity = await connection.QuerySingleAsync<int?>(sql, new { Id = @deckId, cardId = @cardId });
            return quantity ?? 0;
        }


        #region DeckCard Management Methods
        public async Task UpdateDeckCardAsync(IDeckItem deckCard)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"UPDATE {_tableName} SET Quantity = @quantity WHERE DeckId = @deckId AND CardId = @cardId";
            await connection.ExecuteAsync(sql, new { deckId = deckCard.DeckId, cardId = deckCard.CardId, quantity = deckCard.Quantity });
        }

        public async Task DeleteDeckCardAsync(int deckCardId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"DELETE FROM {_tableName} WHERE Id = @deckCardId";
            await connection.ExecuteAsync(sql, new { deckCardId = deckCardId });
        }
        #endregion
        public async Task<int> GetNextDeckId()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT COALESCE(MIN(t.id), MAX(t.id)) + 1 FROM {_tableName} t LEFT JOIN {_tableName} t2 ON t.id = t2.id WHERE t2.id IS NULL";
            int? next_id = await connection.QuerySingleOrDefaultAsync<int?>(sql);
            return next_id ?? 1;
        }

/*
        public async Task<IDeckItem?> GetIdByDeckCardAsync(int deckId, int cardId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE DeckId = @deckId AND CardId = @cardId";
            return await connection.QuerySingleOrDefaultAsync<DeckBuilder?>(sql, new { DeckId = deckId, CardId = cardId });
        }
*/
        public async Task<int> GetNextDeckIdAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT COALESCE(MIN(t1.Id), MAX(t1.Id), 0) + 1 FROM {_tableName} t1 LEFT JOIN {_tableName} t2 ON t1.id + 1 = t2.id WHERE t2.id IS NULL";
            return await connection.QuerySingleOrDefaultAsync<int>(sql);
        }
        
    }
}
