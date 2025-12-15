using Microsoft.EntityFrameworkCore;
using OnlineCasino.Application.DTOs;
using OnlineCasino.Application.Interfaces;
using OnlineCasino.Domain.Entities;
using OnlineCasino.Infrastructure.Data;

namespace OnlineCasino.Application.Services
{
    public class GameService : IGameService
    {
        private readonly CasinoContext _context;

        public GameService(CasinoContext context)
        {
            _context = context;
        }

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games
                .Include(g => g.Bets)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _context.Games.ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetActiveGamesAsync()
        {
            return await _context.Games
                .Where(g => g.IsActive)
                .ToListAsync();
        }

        public async Task<Game> CreateAsync(GameDto dto)
        {
            var game = new Game
            {
                Name = dto.Name,
                Description = dto.Description,
                MinBet = dto.MinBet,
                MaxBet = dto.MaxBet,
                IsActive = dto.IsActive
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return game;
        }

        public async Task<Game?> UpdateAsync(int id, GameDto dto)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return null;

            game.Name = dto.Name;
            game.Description = dto.Description;
            game.MinBet = dto.MinBet;
            game.MaxBet = dto.MaxBet;
            game.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return game;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return false;

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return false;

            game.IsActive = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return false;

            game.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
