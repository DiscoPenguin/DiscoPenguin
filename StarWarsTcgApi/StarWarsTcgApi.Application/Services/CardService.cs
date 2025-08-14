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
    public class CardService : ICardService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;

        public CardService(IUserRepository userRepository, ICardRepository cardRepository)
        {
            _userRepository = userRepository;
            _cardRepository = cardRepository;
        }

        private List<CardInstanceResponse> cardInstanceResponses(IEnumerable<Card> cards)
        {
            var cardSummaries = new List<CardInstanceResponse>();
            foreach (var c in cards)
            {
                cardSummaries.Add(new CardInstanceResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    ExpansionSet = c.ExpansionSet,
                    ImageFile = c.ImageFile,
                    Side = c.Side,
                    Type = c.Type,
                    Subtype = c.Subtype,
                    Cost = c.Cost,
                    Speed = c.Speed,
                    Power = c.Power,
                    Health = c.Health,
                    Rarity = c.Rarity,
                    Number = c.Number,
                    Usage = c.Usage,
                    Text = c.Text,
                    Script = c.Script,
                    Classification = c.Classification
                });
            }
            return cardSummaries;
        }
        public async Task<IEnumerable<CardInstanceResponse>> GetAllCardsAsync()
        {
            var cards = await _cardRepository.GetAllCardsAsync();
            return cardInstanceResponses(cards);
        }
        public async Task<IEnumerable<CardFrequency>> GetFrequentCardsAsync()
        {
            var cards = await _cardRepository.GetFrequentCardsAsync();
            return cards;
        }
        public async Task<Card?> GetCardByIdAsync(int cardId)
        {
            var card = await _cardRepository.GetByIdAsync(cardId);
            return card;
        }

        public async Task<IEnumerable<CardInstanceResponse>> GetCardsByName(string name)
        {
            var cards = await _cardRepository.GetCardsByNameAsync(name);
            return cardInstanceResponses(cards);
        }

        public async Task<IEnumerable<CardInstanceResponse>> GetCardsByType(string type)
        {
            var cards = await _cardRepository.GetCardsByTypeAsync(type);
            return cardInstanceResponses(cards);
        }

        public async Task<IEnumerable<CardInstanceResponse>> GetCardsBySide(string side)
        {
            var cards = await _cardRepository.GetCardsBySideAsync(side);
            return cardInstanceResponses(cards);
        }
    }
}