using System;
using System.Threading.Tasks;
using StarWarsTcgApi.Domain.Models;

namespace StarWarsTcgApi.Domain.Interfaces
{
    public interface IRoleRepository : IRepository<Role, string>
    {
        Task<Role?> GetByNameAsync(string roleName);
        Task<List<Role>> GetAllRolesAsync();
        Task AddRoleAsync(Role role);
        Task<List<Role>> GetRolesByUserIdAsync(string userId);
    }
}