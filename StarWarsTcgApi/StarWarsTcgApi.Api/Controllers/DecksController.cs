using Microsoft.AspNetCore.Mvc;
using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.Interfaces;

namespace StarWarsTcgApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecksController : ControllerBase
    {
        private readonly IDeckService _deckService;

        public DecksController(IDeckService deckService)
        {
            _deckService = deckService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDeck([FromBody] CreateDeckRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var deckResponse = await _deckService.CreateDeckAsync(request);
                if (deckResponse == null)
                {
                    return BadRequest("Failed to create deck. Check input or rules.");
                }
                return CreatedAtAction(nameof(GetDeckById), new { id = deckResponse.DeckId }, deckResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                 return BadRequest(ex.Message); // Deck validation rule violation
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, String.Concat("An unexpected error occurred during deck creation.", ex.Message));
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDeckById(int id)
        {
            var deck = await _deckService.GetDeckByIdAsync(id);
            if (deck == null)
            {
                return NotFound();
            }
            return Ok(deck);
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserDecks(Guid userId)
        {
            var decks = await _deckService.GetUserDecksAsync(userId);
            return Ok(decks);
        }

        [HttpPut("deckCard/{deckId:int}/{cardId:int}/{quantity:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddCardToDeck(int deckId, int cardId, int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("Quantity must be positive.");
            }

            try
            {
                var success = await _deckService.AddCardToDeckAsync(deckId, cardId, quantity);
                if (!success)
                {
                    return NotFound($"Deck {deckId} or Card {cardId} not found, or rule violation.");
                }
                return Ok(new { Message = "Card added/updated in deck successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                 return BadRequest(ex.Message); // Deck validation rule violation
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while adding card to deck.");
            }
        }

        [HttpDelete("{deckId:int}/cards")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveCardFromDeck(int deckId, [FromQuery] int cardId)
        {
            try
            {
                var success = await _deckService.RemoveCardFromDeckAsync(deckId, cardId);
                if (!success)
                {
                    return NotFound($"Deck {deckId} or Card {cardId} not found in deck.");
                }
                return Ok(new { Message = "Card removed/quantity reduced from deck successfully." });
            }
            catch (InvalidOperationException ex)
            {
                 return BadRequest(ex.Message); // Deck validation rule violation (e.g. below min cards)
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while removing card from deck.");
            }
        }
    }
}