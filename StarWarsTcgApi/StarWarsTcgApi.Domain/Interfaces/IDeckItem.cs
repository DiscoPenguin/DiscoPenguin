using StarWarsTcgApi.Domain.Models;

public interface IDeckItem
{
    int? Id { get; set; }
    int DeckId { get; set; }
    int CardId { get; set; }
    int Quantity { get; set; }

}