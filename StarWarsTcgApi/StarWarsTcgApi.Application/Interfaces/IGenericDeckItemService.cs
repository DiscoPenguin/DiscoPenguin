using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using System;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Interfaces
{
    public interface IGenericDeckItemService<T> where T : class, IDeckItem
    {
        Task<IEnumerable<T>> GetCardsInDeckAsync(int deckId);
        Task<T> AddCardToDeckAsync(DeckItemRequest createDto);
        Task<bool> RemoveCardFromDeckAsync(int deckId, int cardId);
        Task<IDeckItem> UpdateCardInDeckAsync(DeckItemRequest updateDto);
        Task<DeckSummaryResponse> CreateDeckAsync(CreateDeckRequest cards);
        Task<bool> DeleteDeck(int deckId);
        Task<int> GetNextDeckIdAsync();
    }
}