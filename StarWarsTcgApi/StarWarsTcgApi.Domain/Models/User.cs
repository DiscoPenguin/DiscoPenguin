using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarWarsTcgApi.Domain.Models
{

    [Table("AspNetUsers", Schema = "SecureAuthDB")]
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public required int AvatarId { get; set; }
        
        [NotMapped]
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}