using System.ComponentModel.DataAnnotations.Schema;

namespace StarWarsTcgApi.Domain.Models
{
    //TODO: Model this to SecureAuthDB.AspNetRoles
    [Table("AspNetUserRoles", Schema = "SecureAuthDB")]
    public class Role
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}