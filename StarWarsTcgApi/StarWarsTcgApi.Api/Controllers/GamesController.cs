using Microsoft.AspNetCore.Mvc;
using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.Interfaces;

namespace StarWarsTcgApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGameStatistics([FromQuery] Guid? userId = null)
        {
            var stats = await _gameService.GetGameStatisticsAsync(userId);
            if (!stats.Any())
            {
                return NotFound("No stats found.");
            }
            return Ok(stats);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var gameResponse = await _gameService.CreateGameAsync(request);
                if (gameResponse == null)
                {
                    // This could happen due to various reasons handled in service, e.g., deck invalid
                    return BadRequest("Failed to create game. Check input data and game rules.");
                }
                return CreatedAtAction(nameof(GetGameById), new { id = gameResponse.GameId }, gameResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Specific validation errors
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Game rule violations
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); // Deck doesn't belong to user
            }
            catch (Exception)
            {
                // Log the exception details
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred during game creation.");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGameById(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return Ok(game);
        }

        [HttpGet("user/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGamesForUser(int userId)
        {
            var games = await _gameService.GetUserGamesAsync(userId);
            return Ok(games);
        }

        [HttpPost("{gameId:int}/playCard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Use Forbidden for unauthorized actions like wrong turn
        public async Task<IActionResult> PlayCard(int gameId, [FromBody] PlayCardRequest request)
        {
            if (gameId != request.GameId)
            {
                return BadRequest("Game ID in route does not match body.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await _gameService.PlayCardAsync(request);
                if (!success)
                {
                    return BadRequest("Failed to play card. Check game state or card validity.");
                }
                return Ok(new { Message = "Card played successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Forbid(ex.Message); // Specific game rule violation (e.g., not your turn, not enough force)
            }
            catch (ArgumentException ex)
            {
                 return BadRequest(ex.Message); // Invalid input for card action
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while playing card.");
            }
        }

        [HttpPost("{gameId:int}/player/{playerId:int}/drawCard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DrawCard(int gameId, int playerId)
        {
             try
            {
                var success = await _gameService.DrawCardAsync(gameId, playerId);
                if (!success)
                {
                    return BadRequest("Failed to draw card. Deck might be empty or action not allowed.");
                }
                return Ok(new { Message = "Card drawn successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while drawing card.");
            }
        }

        [HttpPost("{gameId:int}/player/{playerId:int}/endTurn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> EndTurn(int gameId, int playerId)
        {
            try
            {
                var success = await _gameService.EndTurnAsync(gameId, playerId);
                if (!success)
                {
                    return BadRequest("Failed to end turn. Check game state or player ID.");
                }
                return Ok(new { Message = "Turn ended successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while ending turn.");
            }
        }
    }
}