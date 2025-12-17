using OnlineCasino.Domain.Entities;

namespace OnlineCasino.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction?> GetByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<IEnumerable<Transaction>> GetByPlayerIdAsync(int playerId);
        Task<Transaction> CreateAsync(int playerId, TransactionType type, decimal amount, string? description = null);
        Task<bool> DeleteAsync(int id);
    }
}
