using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.Interfaces;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;
        public AssetService(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }
        private List<AssetInstanceResponse> assetInstanceResponse(IEnumerable<Asset> assets)
        {
            var assetSummaries = new List<AssetInstanceResponse>();
            foreach (var a in assets)
            {
                assetSummaries.Add(new AssetInstanceResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    Url = a.Url,
                    ImageType = a.ImageType
                });
            }
            return assetSummaries;
        }
        public async Task<IEnumerable<AssetInstanceResponse>> GetAllAssetsAsync()
        {
            var assets = await _assetRepository.GetAllAssetsAsync();
            return assetInstanceResponse(assets);
        }
        public async Task<Asset?> GetAssetByIdAsync(int assetId)
        {
            var asset = await _assetRepository.GetByIdAsync(assetId);
            return asset;
        }
    }
}