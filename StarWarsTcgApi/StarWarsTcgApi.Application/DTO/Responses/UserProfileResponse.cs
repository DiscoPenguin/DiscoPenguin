using System;
using System.Diagnostics;

namespace StarWarsTcgApi.Application.DTOs.Responses
{
    //TODO: Realign to match SecureAuthDB.AspNetUsers
    public class UserProfileResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int AvatarId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Expiration { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }
}