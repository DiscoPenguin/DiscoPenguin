namespace StarWarsTcgApi.Domain.Models
{
    public class GamePhase
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int OrderBy { get; set; }
    }
}