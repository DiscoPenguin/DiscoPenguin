using StarWarsTcgApi.Application.DTOs;
using StarWarsTcgApi.Application.Interfaces;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Domain.Models; // For password hashing

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;

namespace StarWarsTcgApi.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration; // To access JWT settings from appsettings.json

        public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
        }

        public async Task<UserProfileResponse?> RegisterAsync(RegisterUserRequest registerDto)
        {
            // 1. Check if username already exists
            if (await _userRepository.UsernameExistsAsync(registerDto.Username))
            {
                return null; // Or throw a specific exception for username taken
            }

            // 2. Hash password
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password, salt);

            // 3. Create User entity
            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = registerDto.Username,
                PasswordHash = passwordHash,
                Email = registerDto.Email,
                AvatarId = 1
            };

            // 4. Assign default role (e.g., "User")
            var defaultRole = await _roleRepository.GetByNameAsync("User");
            if (defaultRole == null)
            {
                // This scenario indicates a missing default role in the database,
                // which should ideally be handled during application startup or initial setup.
                throw new InvalidOperationException("Default 'User' role not found. Please ensure it exists in the database.");
            }

            var roleIds = new List<string> { defaultRole.Id };

            // 5. Add user to database
            await _userRepository.AddWithRolesAsync(newUser, roleIds);

            // 6. Generate JWT for the new user (optional, can be done on separate login)
            var token = GenerateJwtToken(newUser);

            return new UserProfileResponse
            {
                UserName = newUser.UserName,
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                Roles = new List<string> { defaultRole.Name }
            };
        }

        public async Task<UserProfileResponse?> LoginAsync(UserLoginRequest loginDto)
        {
            // 1. Retrieve user by username
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                return null; // User not found
            }

            // 2. Verify password
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null; // Incorrect password
            }

            // 3. Update last login time (optional)
            // user.LastLogin = DateTime.UtcNow;
            // await _userRepository.UpdateAsync(user); // You'd need an UpdateAsync method in IUserRepository

            // 4. Generate JWT
            var token = GenerateJwtToken(user);

            return new UserProfileResponse
            {
                UserName = user?.UserName,
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                Roles = user?.Roles.Select(r => r.Name).ToList() // Include all user roles
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured."));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user?.NormalizedUserName)
            };

            // Add roles as claims
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}