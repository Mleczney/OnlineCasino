using Microsoft.EntityFrameworkCore;
using OnlineCasino.Application.Interfaces;
using OnlineCasino.Domain.Entities;
using OnlineCasino.Infrastructure.Data;

namespace OnlineCasino.Application.Services
{
    public class GameSessionService : IGameSessionService
    {
        private readonly CasinoContext _context;

        public GameSessionService(CasinoContext context)
        {
            _context = context;
        }

        public async Task<GameSession?> GetByIdAsync(int id)
        {
            return await _context.GameSessions
                .Include(gs => gs.Player)
                .Include(gs => gs.Game)
                .Include(gs => gs.Bets)
                .FirstOrDefaultAsync(gs => gs.Id == id);
        }

        public async Task<IEnumerable<GameSession>> GetAllAsync()
        {
            return await _context.GameSessions
                .Include(gs => gs.Player)
                .Include(gs => gs.Game)
                .OrderByDescending(gs => gs.StartedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<GameSession>> GetByPlayerIdAsync(int playerId)
        {
            return await _context.GameSessions
                .Include(gs => gs.Game)
                .Where(gs => gs.PlayerId == playerId)
                .OrderByDescending(gs => gs.StartedAt)
                .ToListAsync();
        }

        public async Task<GameSession> StartSessionAsync(int playerId, int gameId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
                throw new InvalidOperationException("Hráč nenalezen");

            var game = await _context.Games.FindAsync(gameId);
            if (game == null)
                throw new InvalidOperationException("Hra nenalezena");

            var session = new GameSession
            {
                PlayerId = playerId,
                GameId = gameId,
                StartedAt = DateTime.UtcNow,
                InitialBalance = player.Balance
            };

            _context.GameSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<GameSession?> EndSessionAsync(int sessionId)
        {
            var session = await _context.GameSessions
                .Include(gs => gs.Player)
                .Include(gs => gs.Bets)
                .FirstOrDefaultAsync(gs => gs.Id == sessionId);

            if (session == null || session.EndedAt.HasValue)
                return null;

            session.EndedAt = DateTime.UtcNow;
            session.FinalBalance = session.Player?.Balance;
            session.TotalBets = session.Bets?.Count ?? 0;
            session.TotalWagered = session.Bets?.Sum(b => b.Amount) ?? 0;
            session.TotalWon = session.Bets?.Where(b => b.IsWin).Sum(b => b.WinAmount ?? 0) ?? 0;

            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<GameSession?> GetActiveSessionAsync(int playerId, int gameId)
        {
            return await _context.GameSessions
                .FirstOrDefaultAsync(gs => gs.PlayerId == playerId && gs.GameId == gameId && gs.EndedAt == null);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var session = await _context.GameSessions.FindAsync(id);
            if (session == null) return false;

            _context.GameSessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
