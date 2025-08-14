using System.ComponentModel.DataAnnotations.Schema;

namespace StarWarsTcgApi.Domain.Models
{
    public class DeckBuilderCard
    {
        public int Id { get; set; }
        public int DeckBuilderId { get; set; }
        public int CardId { get; set; }
        public int Quantity { get; set; }
        public DeckBuilder DeckBuilder { get; set; }
        public Card Card { get; set; }
    }
}