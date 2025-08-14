using System;
using System.ComponentModel.DataAnnotations;

namespace StarWarsTcgApi.Application.DTOs.Requests
{
    public class PlayCardRequest
    {
        [Required]
        public int GameId { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [Required]
        public int GameCardId { get; set; }

        [Required]
        public int TargetZoneId { get; set; }

        public int TargetGameCardId { get; set; } //Optional for targeting a different card
        
        //NOTE: Add more parameters, as needed (costs paid, choices made, etc)
    }
}