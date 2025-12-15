using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Application.Interfaces;

namespace OnlineCasino.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class GameSessionsController : Controller
    {
        private readonly IGameSessionService _gameSessionService;

        public GameSessionsController(IGameSessionService gameSessionService)
        {
            _gameSessionService = gameSessionService;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _gameSessionService.GetAllAsync();
            return View(sessions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var session = await _gameSessionService.GetByIdAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            return View(session);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var session = await _gameSessionService.GetByIdAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            return View(session);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _gameSessionService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
