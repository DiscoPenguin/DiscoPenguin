using System.ComponentModel.DataAnnotations.Schema;

namespace StarWarsTcgApi.Application.DTOs.Responses
{
    public class PlayerStateResponse
    {
        public int PlayerId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int ForceTotal { get; set; }
        public int BuildPoints { get; set; }

        [NotMapped]
        public int CardsInHandCount { get; set; }
        [NotMapped]
        public int CardsInDeckCount { get; set; }
        [NotMapped]
        public int CardsInDiscardCount { get; set; }

        //NOTE: Add more properties, as needed
    }
}