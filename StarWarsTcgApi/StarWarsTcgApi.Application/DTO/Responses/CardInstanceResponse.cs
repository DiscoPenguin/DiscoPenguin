using System;
namespace StarWarsTcgApi.Application.DTOs.Responses
{

    public class CardInstanceResponse
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string ExpansionSet { get; set; }
        public required string ImageFile { get; set; }
        public required string Side { get; set; }
        public required string Type { get; set; }
        public required string Subtype { get; set; }
        public required string Cost { get; set; }
        public required string Speed { get; set; }
        public required string Power { get; set; }
        public required string Health { get; set; }
        public required string Rarity { get; set; }
        public int Number { get; set; }
        public string? Usage { get; set; }
        public string? Text { get; set; }
        public string? Script { get; set; }
        public string? Classification { get; set; }        
    }
}