namespace StarWarsTcgApi.Domain.Models
{
    public class CardRarity
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Abbreviation { get; set; }
    }
}