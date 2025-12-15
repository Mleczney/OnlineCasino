using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Application.DTOs;
using OnlineCasino.Application.Interfaces;

namespace OnlineCasino.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class GamesController : Controller
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task<IActionResult> Index()
        {
            var games = await _gameService.GetAllAsync();
            return View(games);
        }

        public async Task<IActionResult> Details(int id)
        {
            var game = await _gameService.GetByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _gameService.CreateAsync(dto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(dto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var game = await _gameService.GetByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            var dto = new GameDto
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description,
                MinBet = game.MinBet,
                MaxBet = game.MaxBet,
                IsActive = game.IsActive
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GameDto dto)
        {
            if (id != dto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var game = await _gameService.UpdateAsync(id, dto);
                    if (game == null)
                    {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(dto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var game = await _gameService.GetByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _gameService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
