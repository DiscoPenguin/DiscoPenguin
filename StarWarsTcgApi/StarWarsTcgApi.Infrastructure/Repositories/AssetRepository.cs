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
    public class AssetRepository : GenericRepository<Asset, int>, IAssetRepository
    {
        public AssetRepository(MySqlDataAccess dataAccess) : base(dataAccess, "Assets") { }

        public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName}";
            return await connection.QueryAsync<Asset>(sql);
        }

        public async Task<Asset> GetAssetByIdAsync(int assetId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} where Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Asset>(sql, new { Id = assetId});
        }
    }
}