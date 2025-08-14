using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Interfaces
{
    public interface IDeckService
    {
        Task<DeckSummaryResponse?> CreateDeckAsync(CreateDeckRequest request);
        Task<DeckSummaryResponse?> GetDeckByIdAsync(int deckId);
        Task<IEnumerable<DeckSummaryResponse>> GetUserDecksAsync(Guid userId);
        Task<bool> AddCardToDeckAsync(int deckId, int cardId, int quantity);
        Task<bool> RemoveCardFromDeckAsync(int deckId, int cardId);
    }
}