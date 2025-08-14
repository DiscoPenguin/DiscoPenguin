using System;
using System.ComponentModel.DataAnnotations;

namespace StarWarsTcgApi.Application.DTOs.Requests
{
    public class DeckItemRequest
    {
        public int DeckId { get; set; }
        public int CardId { get; set; }
        public int Quantity { get; set; }
    }
}
