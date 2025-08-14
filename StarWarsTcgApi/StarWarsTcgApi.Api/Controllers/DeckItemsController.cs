using Microsoft.AspNetCore.Mvc;
using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.Interfaces;
using StarWarsTcgApi.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace StarWarsTcgApi.Api.Controllers
{
    // A base controller that can be inherited
    public abstract class BaseDeckItemController<T> : ControllerBase where T : class, IDeckItem, new()
    {
        protected readonly IGenericDeckItemService<T> _service;

        public BaseDeckItemController(IGenericDeckItemService<T> service)
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
        public async Task<ActionResult<IEnumerable<T>>> GetCardsInDeck(int deckId)
        {
            var cards = await _service.GetCardsInDeckAsync(deckId);
            if (!cards.Any())
            {
                return NotFound($"No items found for Deck ID: {deckId}");
            }
            return Ok(cards);
        }

        /// <summary>
        /// Adds a card to a specific deck. If the card already exists, its quantity will be updated.
        /// </summary>
        /// <param name="createDto">The details of the card to add.</param>
        /// <returns>The added or updated deck item.</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<T>> AddCardToDeck([FromBody] DeckItemRequest createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.AddCardToDeckAsync(createDto);
            return CreatedAtAction(nameof(GetCardsInDeck), new { deckId = result.DeckId }, result);
        }

        /// <summary>
        /// Removes a card from a deck by its specific deck item ID.
        /// </summary>
        /// <param name="id">The unique ID of the deck item (not the CardId or DeckId).</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{deckId}/{cardId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RemoveCardFromDeck(int deckId, int cardId)
        {
            var removed = await _service.RemoveCardFromDeckAsync(deckId, cardId);
            if (!removed)
            {
                return NotFound($"Deck item with ID pair {deckId}:{cardId} not found.");
            }
            return NoContent();
        }

        /// <summary>
        /// Updates the quantity of a specific card in a deck.
        /// </summary>
        /// <param name="updateDto">The Deck, Card, and Quantity</param>
        /// <returns>The updated deck item.</returns>
        [HttpPut("")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<T>> UpdateCardInDeck([FromBody] DeckItemRequest updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedItem = await _service.UpdateCardInDeckAsync(updateDto);
            if (updatedItem == null)
            {
                return NotFound($"Deck item with ID pair {updateDto.DeckId}:{updateDto.CardId} not found.");
            }
            return Ok(updatedItem);
        }

        /// <summary>
        /// Creates a new deck with a collection of cards.
        /// </summary>
        /// <param name="cards">Deck initialization and Cards within the Deck</param>
        /// <returns>The created deck</returns>
        [HttpPost("create-deck")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<T>>> CreateDeck([FromBody] CreateDeckRequest cards)
        {
            try
            {
                var newDeck = await _service.CreateDeckAsync(cards);
                if (newDeck == null)
                {
                    return StatusCode(500, "Failed to create new deck.");
                }
                return StatusCode(201, newDeck);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("deck/{deckId}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteDeck(int deckId)
        {
            try
            {
                await _service.DeleteDeck(deckId);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deck/next")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> NextDeckId()
        {
            try
            {
                var nextId = await _service.GetNextDeckIdAsync();
                return Ok(nextId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
