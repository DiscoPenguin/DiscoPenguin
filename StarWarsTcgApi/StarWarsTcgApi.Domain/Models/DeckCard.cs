using System.ComponentModel.DataAnnotations.Schema;

namespace StarWarsTcgApi.Domain.Models
{
    public class DeckCard : IDeckItem
    {
        public int? Id { get; set; }
        public int DeckId { get; set; }
        public int CardId { get; set; }
        public int Quantity { get; set; } = 1;

    }
}
