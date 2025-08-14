using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Interfaces
{
    public interface IDeckBuilderService
    {
        Task<int> CreateDeckBuilderAsync();
        Task<bool> AddCardToDeckBuilderAsync(int deckBuilderId, int cardId, int quantity);
        Task<bool> RemoveCardFromDeckBuilderAsync(int deckBuilderId, int cardId);
        Task<DeckBuilder> GetDeckBuilderAsync(int deckBuilderId);
        Task<int> SaveDeckBuilderAsPermanentDeckAsync(int deckBuilderId, Guid userId, string deckName, string deckDescription);
    }
}