using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.Interfaces;
using StarWarsTcgApi.Application.DTO.Responses;
using StarWarsTcgApi.Domain.Models;
using System.Linq.Expressions;
using StarWarsTcgApi.Application.DTO.Requests;

namespace StarWarsTcgApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCards()
        {
            var cards = await _cardService.GetAllCardsAsync();
            if (!cards.Any())
            {
                return NotFound("No cards found.");
            }
            return Ok(cards);
        }
        [HttpGet("frequent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFrequentCards()
        {
            var cards = await _cardService.GetFrequentCardsAsync();
            if (!cards.Any())
            {
                return NotFound("No cards found.");
            }
            return Ok(cards);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCardById(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null)
            {
                return NotFound($"No cards found by that ID '{id}'");
            }
            return Ok(card);
        }

        private IQueryable<CardInstanceResponse> ApplySearch(IQueryable<CardInstanceResponse> query, string searchTerm, bool useLikeness)
        {
            //TODO: Add more search terms
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (useLikeness)
                {
                    query = query.Where(c =>
                        c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (c.Text != null && c.Text.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        c.ExpansionSet.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                    );
                }
                else
                {
                    query = query.Where(c =>
                        c.Name.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)
                    );
                }
            }
            return query;
        }

        private IQueryable<CardInstanceResponse> ApplySorting(IQueryable<CardInstanceResponse> query, string sortField, string sortDirection)
        {
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                var parameter = Expression.Parameter(typeof(CardInstanceResponse), "card");
                Expression property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<CardInstanceResponse, object>>(Expression.Convert(property, typeof(object)), parameter);

                return sortDirection?.ToLower() == "desc"
                    ? Queryable.OrderByDescending(query, lambda)
                    : Queryable.OrderBy(query, lambda);
            }
            return query.OrderBy(c => c.Name); // Default sort
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResultDto<CardInstanceResponse>>> GetCards([FromQuery] CardSearchRequestDto request)
        {
            if (request.PageNumber < 1)
            {
                return BadRequest("Page number must be greater than 0.");
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                return BadRequest("Page size must be between 1 and 100.");
            }

            try
            {
                var cards = await _cardService.GetAllCardsAsync();
                var query = cards.AsQueryable();

                query = ApplySearch(query, request.SearchTerm, request.UseLikeness ?? true);
                query = ApplySorting(query, request.SortField, request.SortDirection);

                var totalCount = query.Count();
                var items = query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                var pagedResult = new PagedResultDto<CardInstanceResponse>(items, totalCount, request.PageNumber, request.PageSize);
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }


    }
}