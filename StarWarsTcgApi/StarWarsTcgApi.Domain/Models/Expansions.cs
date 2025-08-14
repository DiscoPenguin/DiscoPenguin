namespace StarWarsTcgApi.Domain.Models
{
    public class Expansions
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Abbreviation { get; set; }
	public bool wotc { get; set; } = false;
    }
}
