using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;

namespace StarWarsTcgApi.Application.DTOs.Responses
{
    public class GameDetailsResponse
    {
        public int GameId { get; set; }
        public int DarkPlayerId { get; set; }
        public string DarkPlayerUserName { get; set; } = string.Empty;

        [NotMapped]
        public int LightPlayerId { get; set; }
        public string LightPlayerUserName { get; set; } = string.Empty;

        public string CurrentTurnPlayerUserName { get; set; } = string.Empty;
        public string CurrentPhaseName { get; set; } = string.Empty;
        public string GameStatusName { get; set; } = string.Empty;
        public string? WinnerUserName { get; set; }

        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }

        public PlayerStateResponse? LightPlayerState { get; set; }
        public PlayerStateResponse? DarkPlayerState { get; set; }
    }
}