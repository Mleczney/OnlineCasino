using OnlineCasino.Application.DTOs;
using OnlineCasino.Domain.Entities;

namespace OnlineCasino.Application.Interfaces
{
    public interface IGameService
    {
        Task<Game?> GetByIdAsync(int id);
        Task<IEnumerable<Game>> GetAllAsync();
        Task<IEnumerable<Game>> GetActiveGamesAsync();
        Task<Game> CreateAsync(GameDto dto);
        Task<Game?> UpdateAsync(int id, GameDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);
    }
}
