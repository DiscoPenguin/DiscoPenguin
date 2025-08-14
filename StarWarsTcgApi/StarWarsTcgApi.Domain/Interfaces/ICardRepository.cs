using StarWarsTcgApi.Domain.Models;

namespace StarWarsTcgApi.Domain.Interfaces
{
    public interface ICardRepository : IRepository<Card, int>
    {
        Task<IEnumerable<Card>> GetAllCardsAsync();
        Task<IEnumerable<CardFrequency>> GetFrequentCardsAsync();
        Task<Card> GetCardByIdAsync(int cardId);
        Task<IEnumerable<Card>> GetCardsByNameAsync(string cardName);
        Task<IEnumerable<Card>> GetCardsByTypeAsync(string cardType);
        Task<IEnumerable<Card>> GetCardsBySideAsync(string cardSide);

        #region Static lookup data
        Task<IEnumerable<CardType>> GetAllCardTypesAsync();
        Task<IEnumerable<Side>> GetAllCardSidesAsync();
        #endregion
    }
}