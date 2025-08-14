using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarWarsTcg.Security
{
    public class Role : IdentityRole
    {
        public string? Description { get; set; }
        public Role() : base() { }

        public Role(string roleName) : base(roleName) { }

        // Additional properties can be added here if needed
    }
}