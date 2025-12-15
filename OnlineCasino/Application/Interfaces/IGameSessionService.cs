using OnlineCasino.Domain.Entities;

namespace OnlineCasino.Application.Interfaces
{
    public interface IGameSessionService
    {
        Task<GameSession?> GetByIdAsync(int id);
        Task<IEnumerable<GameSession>> GetAllAsync();
        Task<IEnumerable<GameSession>> GetByPlayerIdAsync(int playerId);
        Task<GameSession> StartSessionAsync(int playerId, int gameId);
        Task<GameSession?> EndSessionAsync(int sessionId);
        Task<GameSession?> GetActiveSessionAsync(int playerId, int gameId);
        Task<bool> DeleteAsync(int id);
    }
}
