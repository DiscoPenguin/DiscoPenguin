namespace StarWarsTcgApi.Domain.Models
{
    public class Side
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required char Abbreviation { get; set; }
    }
}