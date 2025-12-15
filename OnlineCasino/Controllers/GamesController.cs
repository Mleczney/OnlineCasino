using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Application.Interfaces;

namespace OnlineCasino.Controllers
{
    public class GamesController : Controller
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task<IActionResult> Index()
        {
            var games = await _gameService.GetActiveGamesAsync();
            return View(games);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _gameService.GetByIdAsync(id.Value);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }
    }
}
