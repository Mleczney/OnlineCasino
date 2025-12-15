using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Application.Interfaces;

namespace OnlineCasino.Controllers
{
    public class PlayersController : Controller
    {
        private readonly IPlayerService _playerService;

        public PlayersController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public async Task<IActionResult> Index()
        {
            var players = await _playerService.GetAllAsync();
            return View(players);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _playerService.GetByIdAsync(id.Value);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }
    }
}
