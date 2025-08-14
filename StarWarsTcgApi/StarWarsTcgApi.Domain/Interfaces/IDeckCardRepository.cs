using StarWarsTcgApi.Domain.Models;

namespace StarWarsTcgApi.Domain.Interfaces
{
    public interface IDeckCardRepository : IRepository<DeckCard, int>
    {
        Task<IEnumerable<DeckCard>> GetCardsByDeckAsync(int deckId);
        Task<IEnumerable<DeckCard>> GetDecksByCardAsyc(int cardId);
        Task<int> GetDeckCardQuantity(int deckId, int cardId);
        Task UpdateDeckCardAsync(IDeckItem deckCard);
        Task DeleteDeckCardAsync(int deckCardId);
        Task<int> GetNextDeckIdAsync();
        Task<IDeckItem?> GetIdByDeckCardAsync(int deckId, int cardId);
    }
}