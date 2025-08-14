using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.Interfaces;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Domain.Models;
using StarWarsTcgApi.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        public PlayerService(
            IPlayerRepository playerRepository
        ) {
            _playerRepository = playerRepository;
        }
        public async Task<PlayerStateResponse?> GetPlayerById(int id)
        {
            var player = await _playerRepository.GetByIdAsync(id);
            if (player == null) return null;

            return new PlayerStateResponse
            {
                PlayerId = player.Id,
                UserId = Guid.Parse(player.UserId),
                Username = player.Username,
                ForceTotal = player.ForceTotal,
                BuildPoints = player.BuildPoints
            };
        }

    }
}