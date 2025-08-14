using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.Interfaces;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Domain.Models;
using System;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserProfileResponse?> RegisterUserAsync(RegisterUserRequest request)
        {
            // Check for existing username or email
            if (await _userRepository.GetByUsernameAsync(request.Username) != null)
            {
                throw new ArgumentException("Username is already taken.");
            }
            if (await _userRepository.GetByEmailAsync(request.Email) != null)
            {
                throw new ArgumentException("Email is already registered.");
            }

            //TODO: salt & hash the password before storing
            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Username,
                Email = request.Email,
                PasswordHash = request.Password,
                AvatarId = 1
            };

            //TODO: Alter AddAsync to return the new int ID
            await _userRepository.AddAsync(newUser);

            return new UserProfileResponse
            {
                Id = newUser.Id,
                UserName = newUser.UserName,
                Email = newUser.Email
            };
        }

        public async Task<UserProfileResponse?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserProfileResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<UserProfileResponse?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            return new UserProfileResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }
    }
}