using System.ComponentModel.DataAnnotations;

namespace StarWarsTcgApi.Application.DTO.Requests
{
    public class CardSearchRequestDto
    {
        //TODO: Add more search options
        public string? SearchTerm { get; set; }
        public bool? UseLikeness { get; set; } = true;

        //[RegularExpression("^(Name|Rarity|ReleaseDate)$", ErrorMessage = "Invalid sort field.")]
        public string SortField { get; set; } = string.Empty;

        [RegularExpression("^(asc|desc)$", ErrorMessage = "Sort direction must be 'asc' or 'desc'.")]
        public string SortDirection { get; set; } = "asc";

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;
    }
}