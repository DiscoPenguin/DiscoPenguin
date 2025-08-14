using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarWarsTcgApi.Domain.Models
{
    public class CardFrequency
    {
        [Required]
        public string CardId { get; set; } = string.Empty;
        public int Frequency { get; set; }
        public int TotalQuantity { get; set; }
    }
}