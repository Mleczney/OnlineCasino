using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Application.DTOs;
using OnlineCasino.Application.Interfaces;

namespace OnlineCasino.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> Details(int id)
        {
            var player = await _playerService.GetByIdAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return View(player);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterPlayerDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _playerService.CreateAsync(dto);
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
            var player = await _playerService.GetByIdAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            var dto = new PlayerDto
            {
                Id = player.Id,
                Username = player.Username,
                Email = player.Email,
                Balance = player.Balance,
                CreatedAt = player.CreatedAt
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlayerDto dto)
        {
            if (id != dto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var player = await _playerService.UpdateAsync(id, dto);
                    if (player == null)
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
            var player = await _playerService.GetByIdAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return View(player);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _playerService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
