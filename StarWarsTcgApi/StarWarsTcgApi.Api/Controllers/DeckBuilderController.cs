using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.Interfaces;

namespace StarWarsTcgApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeckBuilderController : ControllerBase
    {
        private readonly IDeckBuilderService _deckBuilderService;
        private readonly ILogger<DeckBuilderController> _logger; // Inject logger for better error handling

        public DeckBuilderController(IDeckBuilderService deckBuilderService, ILogger<DeckBuilderController> logger)
        {
            _deckBuilderService = deckBuilderService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new temporary deck builder session.
        /// </summary>
        /// <returns>The ID of the newly created deck builder.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateDeckBuilder()
        {
            try
            {
                int deckBuilderId = await _deckBuilderService.CreateDeckBuilderAsync();
                return Ok(new { DeckBuilderId = deckBuilderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating deck builder.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Adds a card to the specified deck builder.
        /// </summary>
        /// <param name="deckBuilderId">The ID of the deck builder.</param>
        /// <param name="request">Card details to add (CardId, Quantity).</param>
        [HttpPost("{deckBuilderId}/card")]
        public async Task<IActionResult> AddCardToDeckBuilder(int deckBuilderId, [FromBody] AddCardToDeckBuilderRequest request)
        {
            if (request == null || request.CardId <= 0 || request.Quantity <= 0)
            {
                return BadRequest("Invalid card details provided.");
            }

            try
            {
                bool success = await _deckBuilderService.AddCardToDeckBuilderAsync(deckBuilderId, request.CardId, request.Quantity);
                if (success)
                {
                    return Ok("Card added to deck builder successfully.");
                }
                return NotFound("Deck builder or card not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding card to deck builder {DeckBuilderId}.", deckBuilderId);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Removes a card from the specified deck builder.
        /// </summary>
        /// <param name="deckBuilderId">The ID of the deck builder.</param>
        /// <param name="cardId">The ID of the card to remove.</param>
        [HttpDelete("{deckBuilderId}/card/{cardId}")]
        public async Task<IActionResult> RemoveCardFromDeckBuilder(int deckBuilderId, int cardId)
        {
            try
            {
                bool success = await _deckBuilderService.RemoveCardFromDeckBuilderAsync(deckBuilderId, cardId);
                if (success)
                {
                    return Ok("Card removed from deck builder successfully.");
                }
                return NotFound("Deck builder or card not found in deck.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing card {CardId} from deck builder {DeckBuilderId}.", cardId, deckBuilderId);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the current state of a deck builder.
        /// </summary>
        /// <param name="deckBuilderId">The ID of the deck builder.</param>
        /// <returns>The deck builder details including its cards.</returns>
        [HttpGet("{deckBuilderId}")]
        public async Task<IActionResult> GetDeckBuilder(int deckBuilderId)
        {
            try
            {
                var deckBuilder = await _deckBuilderService.GetDeckBuilderAsync(deckBuilderId);
                if (deckBuilder == null)
                {
                    return NotFound("Deck builder not found.");
                }
                return Ok(deckBuilder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving deck builder {DeckBuilderId}.", deckBuilderId);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Saves a temporary deck builder as a permanent deck for an authenticated user.
        /// </summary>
        /// <param name="deckBuilderId">The ID of the deck builder to save.</param>
        /// <param name="request">Deck name and description.</param>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{deckBuilderId}/save")]
        public async Task<IActionResult> SaveDeckBuilder(int deckBuilderId, [FromBody] SaveDeckRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.DeckName))
            {
                return BadRequest("Deck name is required.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("User ID not found in token or invalid format.");
            }

            try
            {
                int permanentDeckId = await _deckBuilderService.SaveDeckBuilderAsPermanentDeckAsync(deckBuilderId, userId, request.DeckName, request.DeckDescription);
                if (permanentDeckId > 0)
                {
                    return Ok(new { PermanentDeckId = permanentDeckId, Message = "Deck saved successfully." });
                }
                return BadRequest("Could not save deck. It might have been saved already or an invalid deck builder ID.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving deck builder {DeckBuilderId} for user {UserId}.", deckBuilderId, userId);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }

    // DTOs for API requests
    // TODO: create these in StarWarsTcgApi.Application/DTOs
    public class AddCardToDeckBuilderRequest
    {
        public int CardId { get; set; }
        public int Quantity { get; set; }
    }

    public class SaveDeckRequest
    {
        public string DeckName { get; set; }
        public string DeckDescription { get; set; }
    }
}
