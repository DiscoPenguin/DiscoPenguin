using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Interfaces
{
    public interface ICardService
    {
        Task<IEnumerable<CardInstanceResponse>> GetAllCardsAsync();
        Task<IEnumerable<CardFrequency>> GetFrequentCardsAsync();
        Task<Card?> GetCardByIdAsync(int cardId);
        Task<IEnumerable<CardInstanceResponse>> GetCardsByName(string name);
        Task<IEnumerable<CardInstanceResponse>> GetCardsByType(string type);
        Task<IEnumerable<CardInstanceResponse>> GetCardsBySide(string side);
    }
}