using System.ComponentModel.DataAnnotations.Schema;

namespace StarWarsTcgApi.Domain.Models
{
    public class Player
    {
        // (PK)
        public required int Id { get; set; }

        // (FK) -> SecureAuthDB.AspNetUsers.Id (varchar(255)/GUID)
        public required string UserId { get; set; }
        public required int ForceTotal { get; set; } = 0;
        public required int BuildPoints { get; set; } = 0;
        public string avatar { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        // (FK) -> swtcg.Game.Id
        [NotMapped]
        public required int GameId { get; set; } = 0;
    }
}
