using StarWarsTcgApi.Domain.Models;

namespace StarWarsTcgApi.Domain.Interfaces
{
    public interface IDeckRepository : IRepository<Deck, int>
    {
        Task DeleteDeckAsync(int deckId);
        Task<IEnumerable<Deck>> GetDecksByUserIdAsync(Guid userId);
        Task<Deck?> GetDeckWithCardsAsync(int deckId);
    }
}