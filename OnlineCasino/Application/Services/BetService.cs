using Microsoft.EntityFrameworkCore;
using OnlineCasino.Application.DTOs;
using OnlineCasino.Application.Interfaces;
using OnlineCasino.Domain.Entities;
using OnlineCasino.Infrastructure.Data;

namespace OnlineCasino.Application.Services
{
    public class BetService : IBetService
    {
        private readonly CasinoContext _context;
        private readonly ITransactionService _transactionService;

        public BetService(CasinoContext context, ITransactionService transactionService)
        {
            _context = context;
            _transactionService = transactionService;
        }

        public async Task<Bet?> GetByIdAsync(int id)
        {
            return await _context.Bets
                .Include(b => b.Player)
                .Include(b => b.Game)
                .Include(b => b.GameSession)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Bet>> GetAllAsync()
        {
            return await _context.Bets
                .Include(b => b.Player)
                .Include(b => b.Game)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bet>> GetByPlayerIdAsync(int playerId)
        {
            return await _context.Bets
                .Include(b => b.Game)
                .Where(b => b.PlayerId == playerId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bet>> GetByGameIdAsync(int gameId)
        {
            return await _context.Bets
                .Include(b => b.Player)
                .Where(b => b.GameId == gameId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Bet> PlaceBetAsync(int playerId, PlaceBetDto dto)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
                throw new InvalidOperationException("Hráč nenalezen");

            var game = await _context.Games.FindAsync(dto.GameId);
            if (game == null)
                throw new InvalidOperationException("Hra nenalezena");

            if (!game.IsActive)
                throw new InvalidOperationException("Hra není aktivní");

            if (dto.Amount < game.MinBet || dto.Amount > game.MaxBet)
                throw new InvalidOperationException($"Sázka musí být mezi {game.MinBet} a {game.MaxBet}");

            if (player.Balance < dto.Amount)
                throw new InvalidOperationException("Nedostatek kreditu");

            // Deduct bet amount from player balance
            player.Balance -= dto.Amount;

            // Simulate game (dice roll)
            var random = new Random();
            int rolled = random.Next(1, 7);
            bool isWin = dto.Guess.HasValue && rolled == dto.Guess.Value;
            decimal? winAmount = null;

            if (isWin)
            {
                winAmount = dto.Amount * 2;
                player.Balance += winAmount.Value;
            }

            var bet = new Bet
            {
                PlayerId = playerId,
                GameId = dto.GameId,
                Amount = dto.Amount,
                WinAmount = winAmount,
                IsWin = isWin,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bets.Add(bet);
            await _context.SaveChangesAsync();

            // Create transaction records
            await _transactionService.CreateAsync(
                playerId,
                TransactionType.BetPlaced,
                dto.Amount,
                $"Sázka na hru {game.Name}"
            );

            if (isWin)
            {
                await _transactionService.CreateAsync(
                    playerId,
                    TransactionType.BetWon,
                    winAmount!.Value,
                    $"Výhra na hře {game.Name}"
                );
            }
            else
            {
                await _transactionService.CreateAsync(
                    playerId,
                    TransactionType.BetLost,
                    dto.Amount,
                    $"Prohra na hře {game.Name}"
                );
            }

            return bet;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var bet = await _context.Bets.FindAsync(id);
            if (bet == null) return false;

            _context.Bets.Remove(bet);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
