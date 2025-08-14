using System;
namespace StarWarsTcgApi.Application.DTOs.Responses
{

    public class AssetInstanceResponse
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Url { get; set; }
        public required string ImageType { get; set; }

    }
}