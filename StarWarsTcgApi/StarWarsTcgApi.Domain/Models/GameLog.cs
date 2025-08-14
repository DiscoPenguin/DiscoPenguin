using System.Text.Json;

namespace StarWarsTcgApi.Domain.Models
{
    public class GameLog
    {
        public required int? Id { get; set; }
        public required int GameId { get; set; }
        public required DateTime Timestamp { get; set; }
        public required int? TurnNumber { get; set; }
        public required int? PhaseId { get; set; }
        public required int? PlayerId { get; set; }
        public required int ActionId { get; set; }
        public required JsonDocument? ActionDetails { get; set; }
    }
}
