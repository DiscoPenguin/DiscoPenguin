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
    public class DeckCardRepository : GenericRepository<DeckCard, int>, IDeckCardRepository
    {
        public DeckCardRepository(MySqlDataAccess dataAccess) : base(dataAccess, "Deck_Cards") { }

        public async Task<IEnumerable<DeckCard>> GetCardsByDeckAsync(int deckId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE DeckId = @deckId";
            return await connection.QueryAsync<DeckCard>(sql, new { DeckId = deckId });
        }

        public async Task<IEnumerable<DeckCard>> GetDecksByCardAsyc(int cardId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE CardId = @cardId";
            return await connection.QueryAsync<DeckCard>(sql, new { CardId = cardId });
        }
        public async Task<int> GetDeckCardQuantity(int deckId, int cardId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT Quantity FROM {_tableName} WHERE DeckId = @deckId AND CardId = @cardId";
            int? quantity = await connection.QuerySingleAsync<int?>(sql, new { deckId = @deckId, cardId = @cardId });
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

        public async Task<IDeckItem?> GetIdByDeckCardAsync(int deckId, int cardId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE DeckId = @deckId AND CardId = @cardId";
            return await connection.QuerySingleOrDefaultAsync<DeckCard?>(sql, new { DeckId = deckId, CardId = cardId });
        }

        public async Task<int> GetNextDeckIdAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT COALESCE(MIN(t1.Id), MAX(t1.Id), 0) + 1 FROM {_tableName} t1 LEFT JOIN {_tableName} t2 ON t1.id + 1 = t2.id WHERE t2.id IS NULL";
            return await connection.QuerySingleOrDefaultAsync<int>(sql);
        }

    }
}
