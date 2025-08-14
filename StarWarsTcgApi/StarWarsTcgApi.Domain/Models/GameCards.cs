namespace StarWarsTcgApi.Domain.Models
{
    public class GameCard
    {
        public required int? Id { get; set; }
        public required int GameId { get; set; }
        public required int CardId { get; set; }
        public required int OwnerPlayerId { get; set; }
        public required int ControllerPlayerId { get; set; }
        public required int CurrentZoneId { get; set; }
        public bool FaceDown { get; set; } = true;
        public bool Tapped { get; set; } = false;
        public int Damage { get; set; } = 0;
        public int? Sequence { get; set; }
    }
}
