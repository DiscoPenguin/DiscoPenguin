using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Interfaces
{
    public interface IAssetService
    {
        Task<IEnumerable<AssetInstanceResponse>> GetAllAssetsAsync();
        Task<Asset?> GetAssetByIdAsync(int assetId);
    }
}