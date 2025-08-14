using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using System;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileResponse?> RegisterUserAsync(RegisterUserRequest request);
        Task<UserProfileResponse?> GetUserByIdAsync(Guid userId);
        Task<UserProfileResponse?> GetUserByUsernameAsync(string username);
        
        //TODO: Add methods for login, update profile, etc.
    }
}