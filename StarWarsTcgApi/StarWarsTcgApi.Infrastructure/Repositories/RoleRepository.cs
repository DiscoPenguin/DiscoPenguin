using System.Data;
using Dapper;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Domain.Models;
using StarWarsTcgApi.Infrastructure.Data;

namespace StarWarsTcgApi.Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role, string>, IRoleRepository
    {

        public RoleRepository(MySqlDataAccess dataAccess) : base(dataAccess, "Role") { }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = "SELECT Id, Name FROM Roles WHERE Name = @RoleName";
            return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { RoleName = roleName });
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = "SELECT Id, Name FROM Roles";
            var roles = await connection.QueryAsync<Role>(sql);
            return roles.ToList();
        }

        public async Task AddRoleAsync(Role role)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = "INSERT INTO Roles (Name) VALUES (@Name)";
            await connection.ExecuteAsync(sql, role);
        }

        public async Task<List<Role>> GetRolesByUserIdAsync(string userId)
        {
            using IDbConnection connection = _dataAccess.GetConnection();
            var sql = @"
                SELECT r.Id, r.Name
                FROM Roles r
                INNER JOIN UserRoles ur ON r.Id = ur.RoleId
                WHERE ur.UserId = @UserId";
            var roles = await connection.QueryAsync<Role>(sql, new { UserId = userId });
            return roles.ToList();
        }
    }
}