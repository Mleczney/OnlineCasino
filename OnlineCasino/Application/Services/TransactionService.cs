using Microsoft.EntityFrameworkCore;
using OnlineCasino.Application.Interfaces;
using OnlineCasino.Domain.Entities;
using OnlineCasino.Infrastructure.Data;

namespace OnlineCasino.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly CasinoContext _context;

        public TransactionService(CasinoContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Player)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions
                .Include(t => t.Player)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByPlayerIdAsync(int playerId)
        {
            return await _context.Transactions
                .Where(t => t.PlayerId == playerId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Transaction> CreateAsync(int playerId, TransactionType type, decimal amount, string? description = null)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
                throw new InvalidOperationException("Hráč nenalezen");

            var transaction = new Transaction
            {
                PlayerId = playerId,
                Type = type,
                Amount = amount,
                BalanceBefore = player.Balance,
                BalanceAfter = player.Balance, // Will be updated based on type
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            // Adjust balance after based on transaction type
            switch (type)
            {
                case TransactionType.Deposit:
                case TransactionType.BetWon:
                    transaction.BalanceAfter = player.Balance + amount;
                    break;
                case TransactionType.Withdrawal:
                case TransactionType.BetPlaced:
                case TransactionType.BetLost:
                    transaction.BalanceAfter = player.Balance - amount;
                    break;
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
