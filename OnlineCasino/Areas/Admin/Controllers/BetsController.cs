using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Application.Interfaces;
using OnlineCasino.Application.DTOs;

namespace OnlineCasino.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class BetsController : Controller
    {
        private readonly IBetService _betService;

        public BetsController(IBetService betService)
        {
            _betService = betService;
        }

        public async Task<IActionResult> Index()
        {
            var bets = await _betService.GetAllAsync();
            var betDtos = bets.Select(b => new BetDto
            {
                Id = b.Id,
                PlayerId = b.PlayerId,
                PlayerUsername = b.Player?.Username ?? "N/A",
                GameId = b.GameId,
                GameName = b.Game?.Name ?? "N/A",
                Amount = b.Amount,
                WinAmount = b.WinAmount,
                IsWin = b.IsWin,
                CreatedAt = b.CreatedAt
            }).ToList();
            return View(betDtos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var bet = await _betService.GetByIdAsync(id);
            if (bet == null)
            {
                return NotFound();
            }
            return View(bet);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var bet = await _betService.GetByIdAsync(id);
            if (bet == null)
            {
                return NotFound();
            }
            return View(bet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _betService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
