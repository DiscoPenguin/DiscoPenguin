using System;
using System.Collections.Generic;

namespace StarWarsTcgApi.Application.DTOs.Responses
{
    public class DeckSummaryResponse
    {
        public int DeckId { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public string DeckName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TotalCards { get; set; }
        public int UniqueCards { get; set; }
        public bool IsPublic { get; set; }

        /// <summary>
        /// Optionally include a list of cards in the deck
        /// </summary>
        public ICollection<DeckCardSummaryResponse>? Cards { get; set; }

    }
}