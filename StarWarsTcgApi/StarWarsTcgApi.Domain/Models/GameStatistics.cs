using System.ComponentModel.DataAnnotations;

namespace StarWarsTcgApi.Domain.Models
{
    public class GameStatistics
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty; //TODO: AvatarId?
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public int TotalGames { get; set; }
        public int LightGames { get; set; }
        public int LightWins { get; set; }
        public int DarkGames { get; set; }
        public int DarkWins { get; set; }
        public int TotalWins { get; set; }
    }
}