using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Data;
using OnlineCasino.Models;

namespace OnlineCasino.Controllers
{
    public class BalanceDepositController : Controller
    {
        private readonly CasinoContext _context;

        public BalanceDepositController(CasinoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult DepositBalance()
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var player = _context.Players.Find(playerId.Value);
            if (player == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Username = player.Username;
            ViewBag.CurrentBalance = player.Balance;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DepositBalance(decimal depositAmount)
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (depositAmount <= 0)
            {
                ViewBag.Message = "Částka musí být větší než 0 💸";
                return View();
            }

            var player = await _context.Players.FindAsync(playerId.Value);
            if (player == null)
            {
                return RedirectToAction("Login", "Login");
            }

            player.Balance += depositAmount;
            await _context.SaveChangesAsync();

            HttpContext.Session.SetDecimal("Balance", player.Balance);

            ViewBag.Message = $"✅ Úspěšně připsáno {depositAmount} Kč!";
            ViewBag.Username = player.Username;
            ViewBag.CurrentBalance = player.Balance;

            return View();
        }
    }
}
