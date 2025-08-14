using Microsoft.AspNetCore.Identity;

namespace StarWarsTcg.Security
{
    public class User : IdentityUser
    {
        public int AvatarId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}