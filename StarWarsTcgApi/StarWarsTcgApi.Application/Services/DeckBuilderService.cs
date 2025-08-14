using StarWarsTcgApi.Application.Interfaces;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Services
{
    public class DeckBuilderService : IDeckBuilderService
    {
        private readonly IDeckBuilderRepository _deckBuilderRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IDeckRepository _deckRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDeckCardRepository _deckCardRepository;

        public DeckBuilderService(
            IDeckBuilderRepository deckBuilderRepository,
            IDeckCardRepository deckCardRepository,
            ICardRepository cardRepository,
            IDeckRepository deckRepository,
            IUserRepository userRepository)
        {
            _deckBuilderRepository = deckBuilderRepository;
            _cardRepository = cardRepository;
            _deckRepository = deckRepository;
            _userRepository = userRepository;
        }

        public async Task<int> CreateDeckBuilderAsync()
        {
            var deckBuilder = new DeckBuilder
            {
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
                IsSaved = false
            };
            await _deckBuilderRepository.AddAsync(deckBuilder);
            return deckBuilder.Id.Value; // The ID of the newly created DeckBuilder
        }

        public async Task<bool> AddCardToDeckBuilderAsync(int deckBuilderId, int cardId, int quantity)
        {
            var deckBuilder = await _deckBuilderRepository.GetByIdAsync(deckBuilderId);
            if (deckBuilder == null)
            {
                return false; // DeckBuilder not found
            }

            // Assuming CardRepository can validate cardId
            var card = await _cardRepository.GetByIdAsync(cardId);
            if (card == null)
            {
                return false; // Card not found
            }

            var existingCard = deckBuilder.DeckBuilderCards.FirstOrDefault(dbc => dbc.CardId == cardId);

            if (existingCard != null)
            {
                // Update quantity if card already exists
                existingCard.Quantity += quantity;
                await _deckBuilderRepository.UpdateAsync(deckBuilder); // Update the DeckBuilder which contains the updated card
            }
            else
            {
                // Add new card
                var newDeckBuilderCard = new DeckBuilderCard
                {
                    DeckBuilderId = deckBuilderId,
                    CardId = cardId,
                    Quantity = quantity
                };
                deckBuilder.DeckBuilderCards.Add(newDeckBuilderCard);
                await _deckBuilderRepository.UpdateAsync(deckBuilder); // Add the new card to the DeckBuilder and update
            }

            deckBuilder.LastModifiedAt = DateTime.UtcNow;
            await _deckBuilderRepository.UpdateAsync(deckBuilder);

            return true;
        }

        public async Task<bool> RemoveCardFromDeckBuilderAsync(int deckBuilderId, int cardId)
        {
            var deckBuilder = await _deckBuilderRepository.GetByIdAsync(deckBuilderId);
            if (deckBuilder == null)
            {
                return false; // DeckBuilder not found
            }

            var existingCard = deckBuilder.DeckBuilderCards.FirstOrDefault(dbc => dbc.CardId == cardId);

            if (existingCard != null)
            {
                deckBuilder.DeckBuilderCards.Remove(existingCard);
                deckBuilder.LastModifiedAt = DateTime.UtcNow;
                await _deckBuilderRepository.UpdateAsync(deckBuilder); // Remove the card and update the DeckBuilder
                return true;
            }

            return false; // Card not found in DeckBuilder
        }

        public async Task<DeckBuilder> GetDeckBuilderAsync(int deckBuilderId)
        {
            return await _deckBuilderRepository.GetByIdAsync(deckBuilderId);
        }

        public async Task<int> SaveDeckBuilderAsPermanentDeckAsync(int deckBuilderId, Guid userId, string deckName, string deckDescription)
        {
            var deckBuilder = await _deckBuilderRepository.GetByIdAsync(deckBuilderId);
            if (deckBuilder == null || deckBuilder.IsSaved)
            {
                return 0; // DeckBuilder not found or already saved
            }

            // Validate user
            var user = await _userRepository.GetByIdAsync(userId); // Assuming UserRepository exists
            if (user == null)
            {
                return 0; // User not found
            }
            
            // Create new permanent Deck
            var permanentDeck = new Deck // Assuming Deck entity exists
            {
                Name = deckName,
                Description = deckDescription,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow, // Assuming CreatedAt exists on Deck
                LastUpdated = DateTime.UtcNow // Assuming LastModifiedAt exists on Deck
            };

            await _deckRepository.AddAsync(permanentDeck); // Assuming DeckRepository exists

            // Add cards to Deck_Cards table
            foreach (var deckBuilderCard in deckBuilder.DeckBuilderCards)
            {
                var deckCard = new DeckCard // Assuming DeckCard entity exists
                {
                    DeckId = permanentDeck.Id.Value,
                    CardId = deckBuilderCard.CardId,
                    Quantity = deckBuilderCard.Quantity
                };
                await _deckCardRepository.AddAsync(deckCard);
            }

            // Update DeckBuilder
            deckBuilder.IsSaved = true;
            deckBuilder.DeckId = permanentDeck.Id;
            deckBuilder.LastModifiedAt = DateTime.UtcNow;
            await _deckBuilderRepository.UpdateAsync(deckBuilder);

            return permanentDeck.Id.Value;
        }
    }
}
