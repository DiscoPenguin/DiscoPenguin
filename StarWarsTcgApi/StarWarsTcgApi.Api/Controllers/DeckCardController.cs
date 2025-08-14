using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.Interfaces;
using StarWarsTcgApi.Application.Services;
using StarWarsTcgApi.Domain.Models;

namespace StarWarsTcgApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeckCardsController : ControllerBase
    {
        protected readonly DeckCardService _service;
        public DeckCardsController(DeckCardService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all cards within a specific deck.
        /// </summary>
        /// <param name="deckId">The ID of the deck.</param>
        /// <returns>A list of deck items.</returns>
        [HttpGet("deck/{deckId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Deck>>> GetCardsInDeck(int deckId)
        {
            var deck = await _service.GetDeckByIdAsync(deckId);
            if (deck == null)
            {
                return NotFound($"No items found for Deck ID: {deckId}");
            }
            return Ok(deck);
        }   
    }
}