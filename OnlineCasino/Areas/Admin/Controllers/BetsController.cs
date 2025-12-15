using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Application.Interfaces;

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
            return View(bets);
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
