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
    public class CardRepository : GenericRepository<Card, int>, ICardRepository
    {
        public CardRepository(MySqlDataAccess dataAccess) : base(dataAccess, "wotc") { }

        public async Task<IEnumerable<Card>> GetAllCardsAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName}";
            return await connection.QueryAsync<Card>(sql);            
        }
        public async Task<IEnumerable<CardFrequency>> GetFrequentCardsAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM swtcg.vwMostChosenCards";
            return await connection.QueryAsync<CardFrequency>(sql);         
        }
        public async Task<Card> GetCardByIdAsync(int cardId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Card>(sql, new { Id = cardId });
        }
        public async Task<IEnumerable<Card>> GetCardsByNameAsync(string cardname)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE LOWER(Name) LIKE LOWER(@likeness)";
            string likeness = string.Concat("%", cardname, "%");
            return await connection.QueryAsync<Card>(sql, new { likeness = likeness });
        }

        public async Task<IEnumerable<Card>> GetCardsByTypeAsync(string cardType)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE Type = @Type";
            return await connection.QueryAsync<Card>(sql, new { Type = cardType });
        }

        public async Task<IEnumerable<Card>> GetCardsBySideAsync(string cardSide)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE Side = @cardSide";
            return await connection.QueryAsync<Card>(sql, new { cardSide = cardSide });
        }

        #region Card FK Lookups
        public async Task<IEnumerable<CardType>> GetAllCardTypesAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM types";
            return await connection.QueryAsync<CardType>(sql);
        }

        public async Task<IEnumerable<Side>> GetAllCardSidesAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM sides";
            return await connection.QueryAsync<Side>(sql);
        }
        #endregion
    }

}