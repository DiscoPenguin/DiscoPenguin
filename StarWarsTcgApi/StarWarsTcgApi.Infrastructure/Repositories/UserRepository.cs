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
    public class UserRepository : GenericRepository<User, Guid>, IUserRepository
    {
        public UserRepository(MySqlDataAccess dataAccess) : base(dataAccess, "SecureAuthDB.AspNetUsers") { }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE Name = @Username";
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE Email = @Email";
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<string> AddWithRolesAsync(User user, List<string> roleIds)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            //NOTE: TRANSACTION PROCESSING BLOCK
            using var transaction = connection.BeginTransaction();
            try
            {
                // Insert User
                var userSql = @"
                    INSERT INTO Users (Username, PasswordHash, Salt, Email, DateCreated)
                    VALUES (@Username, @PasswordHash, @Salt, @Email, @DateCreated);
                    SELECT LAST_INSERT_ID();"; // Get the ID of the newly inserted user

                var userId = await connection.ExecuteScalarAsync<Guid>(userSql, user, transaction: transaction);
                user.Id = userId.ToString();

                // Insert UserRoles
                if (roleIds != null && roleIds.Any())
                {
                    var userRoleSql = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);";
                    var userRoles = roleIds.Select(roleId => new { UserId = userId, RoleId = roleId });
                    await connection.ExecuteAsync(userRoleSql, userRoles, transaction: transaction);
                }

                transaction.Commit();
                return user.Id;
            }
            catch
            {
                transaction.Rollback();
                throw; // Re-throw the exception after rollback
            }
        }

        public override async Task UpdateAsync(User user)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = $"UPDATE {_tableName} SET UserName = @UserName, Email = @Email, AvatarId = @AvatarId WHERE Id = @Id";
            await connection.ExecuteAsync(sql, user);
        }

        public Task DeleteAsync(string id)
        {
            //NOTE: DeleteAsync should not be possible
            throw new NotSupportedException();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            var user = await this.GetByUsernameAsync(username);
            return (user != null);
        }
    }
}