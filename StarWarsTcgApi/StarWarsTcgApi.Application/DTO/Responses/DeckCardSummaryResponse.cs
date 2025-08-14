using System;
namespace StarWarsTcgApi.Application.DTOs.Responses
{

    public class DeckCardSummaryResponse
    {
        public int CardId { get; set; }
        public string CardName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}