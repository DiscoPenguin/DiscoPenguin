using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.Interfaces;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Services
{
    public class DeckCardService : IDeckService
    {
        private readonly IDeckRepository _deckRepository;
        private readonly IDeckCardRepository _deckCardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ICardRepository _cardRepository;

        public DeckCardService(
            IDeckRepository deckRepository,
            IDeckCardRepository deckCardRepository,
            IUserRepository userRepository,
            IPlayerRepository playerRepository,
            ICardRepository cardRepository
        )
        {
            _deckRepository = deckRepository;
            _deckCardRepository = deckCardRepository;
            _userRepository = userRepository;
            _playerRepository = playerRepository;
            _cardRepository = cardRepository;
        }

        public async Task<DeckSummaryResponse?> CreateDeckAsync(CreateDeckRequest request)
        {
            var player = await _userRepository.GetByIdAsync(request.CreatedBy);
            if (player == null)
            {
                throw new ArgumentException("User Id {request.CreatedBy} was not found.");
            }

            // Optional: Check for duplicate deck names for the same user
            // TODO: Add more validation

            var newDeck = new Deck
            {
                Id = null,
                Name = request.DeckName,
                Description = request.Description,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                IsValid = false
            };
            int newDeckId = await _deckRepository.AddAsync(newDeck);

            // If initial cards are provided, add them
            if (request.InitialCards != null && request.InitialCards.Any())
            {
                foreach (var entry in request.InitialCards)
                {
                    var card = await _cardRepository.GetByIdAsync(entry.Key);
                    if (card == null)
                    {
                        throw new ArgumentException($"Card with ID {entry.Key} not found for initial deck population.");
                    }
                    if (entry.Value <= 0 || entry.Value > 4) // Enforce 4-of rule here too
                    {
                         throw new ArgumentException($"Invalid quantity for card {card.Name}. Must be between 1 and 4.");
                    }
                    var newDeckCard = new DeckCard
                    {
                        Id = null,
                        DeckId = newDeckId,
                        CardId = card.Id,
                        Quantity = entry.Value
                    };
                    int newDeckCardId = await _deckCardRepository.AddAsync(newDeckCard);
                }
            }

            return await GetDeckByIdAsync(newDeckId);
        }

        public async Task<DeckSummaryResponse?> GetDeckByIdAsync(int deckId)
        {
            var deck = await _deckRepository.GetDeckWithCardsAsync(deckId);
            if (deck == null) return null;

            var createdByUser = await _userRepository.GetByIdAsync(deck.CreatedBy);
            if (createdByUser == null) return null;

            var deckCardSummaries = new List<DeckCardSummaryResponse>();
            if (deck.DeckCards != null)
            {
                foreach (var dc in deck.DeckCards)
                {
                    var card = await _cardRepository.GetByIdAsync(dc.CardId);
                    if (card != null)
                    {
                        deckCardSummaries.Add(new DeckCardSummaryResponse
                        {
                            CardId = card.Id,
                            CardName = card.Name,
                            Quantity = dc.Quantity
                        });
                    }
                }
            }

            return new DeckSummaryResponse
            {
                DeckId = deckId,
                CreatedBy = deck.CreatedBy,
                CreatedByUserName = createdByUser.UserName,
                DeckName = deck.Name,
                Description = deck.Description,
                TotalCards = deckCardSummaries.Sum(dcs => dcs.Quantity),
                UniqueCards = deckCardSummaries.Count,
                Cards = deckCardSummaries
            };
        }

        public async Task<IEnumerable<DeckSummaryResponse>> GetUserDecksAsync(Guid userId)
        {
            var decks = await _deckRepository.GetDecksByUserIdAsync(userId);
            var deckSummaries = new List<DeckSummaryResponse>();
            foreach (var deck in decks)
            {
                var summary = await GetDeckByIdAsync(deck.Id.Value);
                if (summary != null)
                {
                    deckSummaries.Add(summary);
                }
            }
            return deckSummaries;
        }

        public async Task<bool> AddCardToDeckAsync(int deckId, int cardId, int quantity)
        {
            var deck = await _deckRepository.GetDeckWithCardsAsync(deckId);
            if (deck == null) return false;

            var cardDefinition = await _cardRepository.GetByIdAsync(cardId);
            if (cardDefinition == null) return false;

            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");

            var existingDeckCard = deck.DeckCards?.FirstOrDefault(dc => dc.CardId == cardId);

            if (existingDeckCard != null)
            {
                if ((existingDeckCard.Quantity + quantity) > 4)
                {
                    throw new InvalidOperationException($"Adding {quantity} copies of {cardDefinition.Name} would exceed the 4-copy limit.");
                }
                existingDeckCard.Quantity += quantity;
                await _deckCardRepository.UpdateDeckCardAsync(existingDeckCard);
            }
            else
            {
                // Check 4-of rule for new additions
                if (quantity > 4)
                {
                     throw new InvalidOperationException($"Cannot add {quantity} copies of {cardDefinition.Name} at once, exceeds 4-copy limit.");
                }
                await _deckCardRepository.AddAsync(new DeckCard
                {
                    Id = null,
                    DeckId = deckId,
                    CardId = cardId,
                    Quantity = quantity
                });
            }

            return true;
        }

        public async Task<bool> RemoveCardFromDeckAsync(int deckId, int cardId)
        {
            var deck = await _deckRepository.GetDeckWithCardsAsync(deckId);
            if (deck == null) return false;

            var existingDeckCard = deck.DeckCards?.FirstOrDefault(dc => dc.CardId == cardId);
            if (existingDeckCard == null) return false; // Card not in deck

            if (existingDeckCard.Quantity > 1)
            {
                existingDeckCard.Quantity--;
                await _deckCardRepository.UpdateDeckCardAsync(existingDeckCard);
            }
            else
            {
                await _deckCardRepository.DeleteDeckCardAsync(existingDeckCard.Id.Value);
            }

            return true;
        }
    }
}