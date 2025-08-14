namespace StarWarsTcgApi.Domain.Models
{
    // swtcg.Assets
    public class Asset
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Url { get; set; }
        public required string ImageType { get; set; }
    }
}