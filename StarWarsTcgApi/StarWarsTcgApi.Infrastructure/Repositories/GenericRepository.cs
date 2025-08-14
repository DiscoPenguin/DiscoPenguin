using System.Data;
using Dapper;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Infrastructure.Data;
using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarWarsTcgApi.Infrastructure.Repositories
{
    public abstract class GenericRepository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class
    {
        // Virtual methods are used here in the event a more specific repository needs to override the default behavior
        protected readonly MySqlDataAccess _dataAccess;
        protected readonly string _tableName;

        public GenericRepository(MySqlDataAccess dataAccess, string tableName)
        {
            _dataAccess = dataAccess;
            _tableName = tableName;
        }

        public virtual async Task<TEntity?> GetByIdAsync(TId id)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<TEntity>(sql, new { Id = id });
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName}";
            return await connection.QueryAsync<TEntity>(sql);
        }

        public virtual async Task<TId> AddAsync(TEntity entity)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var properties = typeof(TEntity).GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0)
                .Where(p => p.Name != "Id" || (p.Name == "Id" && p.PropertyType != typeof(int) && p.PropertyType != typeof(int?)))
                .Select(p => p.Name);
            var columns = string.Join(", ", properties);
            var parameters = string.Join(", ", properties.Select(p => $"@{p}"));
            var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({parameters}); SELECT LAST_INSERT_ID();";
            // If the entity's ID is a Guid, ensure it's assigned before insertion for Dapper to pick it up
            var newObjectId = await connection.QuerySingleAsync<TId>(sql, entity);
            return newObjectId;
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var properties = typeof(TEntity).GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0)
                .Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)); // DO NOT UPDATE THE PK
            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
            var sql = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";
            await connection.ExecuteAsync(sql, entity);
        }

        public async Task DeleteAsync(TId id)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"DELETE FROM {_tableName} WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}