using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineCasino.Application.DTOs;
using OnlineCasino.Application.Interfaces;

namespace OnlineCasino.Controllers
{
    public class BetsController : Controller
    {
        private readonly IBetService _betService;
        private readonly IGameService _gameService;

        public BetsController(IBetService betService, IGameService gameService)
        {
            _betService = betService;
            _gameService = gameService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var bets = await _betService.GetByPlayerIdAsync(playerId.Value);
            return View(bets);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Play()
        {
            var games = await _gameService.GetActiveGamesAsync();
            ViewBag.Games = new SelectList(games, "Id", "Name");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Play(PlaceBetDto dto)
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var bet = await _betService.PlaceBetAsync(playerId.Value, dto);

                    // Update session balance
                    var player = await _betService.GetByIdAsync(bet.Id);

                    ViewBag.Bet = bet;
                    ViewBag.Message = bet.IsWin ? "🎉 Vyhráli jste!" : "💀 Prohráli jste!";

                    return View("Result");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var games = await _gameService.GetActiveGamesAsync();
            ViewBag.Games = new SelectList(games, "Id", "Name");
            return View(dto);
        }


    }
}
