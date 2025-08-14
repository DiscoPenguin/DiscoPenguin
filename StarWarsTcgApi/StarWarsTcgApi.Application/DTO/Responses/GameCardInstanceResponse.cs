using System;
namespace StarWarsTcgApi.Application.DTOs.Responses
{
    public class GameCardInstanceResponse
    {
        public int? GameCardId { get; set; }
        public int CardId { get; set; } // from wotc.Id ("static" card definition)
        public string CardName { get; set; } = string.Empty;
        public string CardTypeName { get; set; } = string.Empty;
        public string CardSideName { get; set; } = string.Empty;
        public string? OwnerUserName { get; set; } = string.Empty;
        public string? ControllerUserName { get; set; } = string.Empty;
        public string? ControllerZoneName { get; set; } = string.Empty;

        public int? SequenceInZone { get; set; }

        public bool Tapped { get; set; }
        public bool FaceDown { get; set; }
        public int DamageCounters { get; set; } = 0;
        public string Power { get; set; } = string.Empty;
        public int? DeployCost { get; set; }

        //NOTE: Add more properties, as needed
    }
}