using System.ComponentModel.DataAnnotations.Schema;

namespace StarWarsTcgApi.Domain.Models
{
    public class Deck
    {
        public int? Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsValid { get; set; }
        public bool IsPublic { get; set; }

        //Navigation property for Deck_Cards
        //Optional, but useful for ORMs like EF Core
        [NotMapped]
        public ICollection<IDeckItem>? DeckCards { get; set; }
    }
}