using Microsoft.AspNetCore.Mvc;
using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.Interfaces;

namespace StarWarsTcgApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;

        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
            /*
                BaseDirectory: /home/ralph/Documents/StarWarsTcg/StarWarsTcgApi/StarWarsTcgApi.Api/bin/Debug/net8.0/
                WebRootPath: 
                ContentRootPath: /home/ralph/Documents/StarWarsTcg/StarWarsTcgApi/StarWarsTcgApi.Api
            */
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAssets()
        {
            var assets = await _assetService.GetAllAssetsAsync();
            if (!assets.Any())
            {
                return NotFound("No assets found.");
            }
            return Ok(assets);
        }
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAssetById(int id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null)
            {
                return NotFound($"No asset found by ID '{id}'");
            }
            return Ok(asset);
        }
    }
}