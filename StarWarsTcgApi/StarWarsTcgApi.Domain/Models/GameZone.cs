namespace StarWarsTcgApi.Domain.Models
{
    public class GameZone
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public bool CanHaveLocation { get; set; } = false;
    }
}