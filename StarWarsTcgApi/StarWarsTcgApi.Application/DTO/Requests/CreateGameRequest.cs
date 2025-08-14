using System;
using System.ComponentModel.DataAnnotations;

namespace StarWarsTcgApi.Application.DTOs.Requests
{
    public class CreateGameRequest
    {
        [Required]
        public Guid LightUserId { get; set; } = Guid.Empty;
        [Required]
        public int LightPlayerId { get; set; }
        [Required]
        public int LightPlayerDeckId { get; set; }
        [Required]
        public Guid DarkUserId { get; set; } = Guid.Empty;
        [Required]
        public int DarkPlayerId { get; set; }
        [Required]
        public int DarkPlayerDeckId { get; set; }
    }
}