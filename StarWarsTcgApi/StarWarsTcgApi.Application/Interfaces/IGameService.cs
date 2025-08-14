using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Interfaces
{
    public interface IGameService
    {
        Task<IEnumerable<GameStatistics>> GetGameStatisticsAsync(Guid? userId);
        Task<GameDetailsResponse?> CreateGameAsync(CreateGameRequest request);
        Task<GameDetailsResponse?> GetGameByIdAsync(int gameId);
        Task<IEnumerable<GameDetailsResponse>> GetUserGamesAsync(int userId);
        Task<bool> PlayCardAsync(PlayCardRequest request);
        Task<bool> DrawCardAsync(int gameId, int playerId);
        Task<bool> EndTurnAsync(int gameId, int currentPlayerId);
        
        //TODO: Create more methods to match the game actions
    }
}