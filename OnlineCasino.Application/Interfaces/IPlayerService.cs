using OnlineCasino.Application.DTOs;
using OnlineCasino.Domain.Entities;

namespace OnlineCasino.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<Player?> GetByIdAsync(int id);
        Task<Player?> GetByUsernameAsync(string username);
        Task<IEnumerable<Player>> GetAllAsync();
        Task<Player> CreateAsync(RegisterPlayerDto dto);
        Task<Player?> UpdateAsync(int id, PlayerDto dto);
        Task<bool> DeleteAsync(int id);
        Task<Player?> AuthenticateAsync(string username, string password);
        Task<bool> DepositAsync(int playerId, decimal amount);
        Task<bool> WithdrawAsync(int playerId, decimal amount);
        Task<decimal> GetBalanceAsync(int playerId);
    }
}
