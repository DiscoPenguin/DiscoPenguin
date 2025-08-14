using System;
using System.ComponentModel.DataAnnotations;

namespace StarWarsTcgApi.Application.DTOs.Requests
{
    public class CreateDeckRequest
    {
        /// <summary>
        /// CreatedBy
        /// </summary>
        [Required]
        public Guid CreatedBy { get; set; } = Guid.Empty;

        [Required]
        [StringLength(255)]
        public string DeckName { get; set; } = string.Empty;

        public string? Description { get; set; }
        public Boolean IsPublic { get; set; } = true;

        /// <summary>
        /// Optional method allowing for bulk creation of a Deck with the assigned card and quantity
        /// CardId, Quantity
        /// </summary>
        public Dictionary<int, int>? InitialCards { get; set; }
    }
}