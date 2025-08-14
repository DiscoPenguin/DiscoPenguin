using System;
using System.Threading.Tasks;
using StarWarsTcgApi.Domain.Models;

namespace StarWarsTcgApi.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User, Guid>
    {
        Task<string> AddWithRolesAsync(User user, List<string> roleIds);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
        Task<User?> GetByEmailAsync(string email);
    }
}