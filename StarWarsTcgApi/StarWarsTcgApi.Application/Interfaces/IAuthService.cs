using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserProfileResponse?> RegisterAsync(RegisterUserRequest registerDto);
        Task<UserProfileResponse?> LoginAsync(UserLoginRequest loginDto);
    }

}