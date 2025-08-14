using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using System;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<PlayerStateResponse?> GetPlayerById(int id);

    }
}