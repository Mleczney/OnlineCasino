using OnlineCasino.Application.DTOs;
using OnlineCasino.Domain.Entities;

namespace OnlineCasino.Application.Interfaces
{
    public interface IBetService
    {
        Task<Bet?> GetByIdAsync(int id);
        Task<IEnumerable<Bet>> GetAllAsync();
        Task<IEnumerable<Bet>> GetByPlayerIdAsync(int playerId);
        Task<IEnumerable<Bet>> GetByGameIdAsync(int gameId);
        Task<Bet> PlaceBetAsync(int playerId, PlaceBetDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
