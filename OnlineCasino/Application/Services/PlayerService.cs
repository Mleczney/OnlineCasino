using Microsoft.EntityFrameworkCore;
using OnlineCasino.Application.DTOs;
using OnlineCasino.Application.Interfaces;
using OnlineCasino.Domain.Entities;
using OnlineCasino.Infrastructure.Data;

namespace OnlineCasino.Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly CasinoContext _context;
        private readonly ITransactionService _transactionService;

        public PlayerService(CasinoContext context, ITransactionService transactionService)
        {
            _context = context;
            _transactionService = transactionService;
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _context.Players
                .Include(p => p.Bets)
                .Include(p => p.Transactions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Player?> GetByUsernameAsync(string username)
        {
            return await _context.Players
                .FirstOrDefaultAsync(p => p.Username == username);
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<Player> CreateAsync(RegisterPlayerDto dto)
        {
            var player = new Player
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Balance = 1000,
                CreatedAt = DateTime.UtcNow
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            // Create initial balance transaction
            await _transactionService.CreateAsync(player.Id, TransactionType.Deposit, 1000, "Počáteční kredit");

            return player;
        }

        public async Task<Player?> UpdateAsync(int id, PlayerDto dto)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return null;

            player.Username = dto.Username;
            player.Email = dto.Email;
            player.Balance = dto.Balance;

            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return false;

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Player?> AuthenticateAsync(string username, string password)
        {
            var player = await GetByUsernameAsync(username);
            if (player == null) return null;

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, player.PasswordHash);
            return isValidPassword ? player : null;
        }

        public async Task<bool> DepositAsync(int playerId, decimal amount)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null) return false;

            var balanceBefore = player.Balance;
            player.Balance += amount;
            await _context.SaveChangesAsync();

            await _transactionService.CreateAsync(playerId, TransactionType.Deposit, amount, "Vklad");

            return true;
        }

        public async Task<bool> WithdrawAsync(int playerId, decimal amount)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null || player.Balance < amount) return false;

            var balanceBefore = player.Balance;
            player.Balance -= amount;
            await _context.SaveChangesAsync();

            await _transactionService.CreateAsync(playerId, TransactionType.Withdrawal, amount, "Výběr");

            return true;
        }

        public async Task<decimal> GetBalanceAsync(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            return player?.Balance ?? 0;
        }
    }
}
