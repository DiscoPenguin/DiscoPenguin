using System.ComponentModel.DataAnnotations.Schema;

namespace StarWarsTcgApi.Domain.Models
{
    [Table("DeckBuilder", Schema = "swtcg")]
    public class DeckBuilder
    {
        public int? Id { get; set; }
        public int? DeckId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public bool IsSaved { get; set; }
        [NotMapped]
        public ICollection<DeckBuilderCard> DeckBuilderCards { get; set; } = new List<DeckBuilderCard>();

    }
}