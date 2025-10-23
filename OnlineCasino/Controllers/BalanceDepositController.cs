using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineCasino.Models;

namespace OnlineCasino.Controllers
{
    public class BalanceDepositController : Controller
    {
        private readonly ILogger<BalanceDepositController> _logger;

        // Simulace databáze (v reálné aplikaci by se použila služba nebo DbContext)
        private static List<Player> _players = new List<Player>
        {
            new Player { Id = 1, Username = "Tomáš", Balance = 100.00m },
            new Player { Id = 2, Username = "Anna", Balance = 250.50m }
        };

        public BalanceDepositController(ILogger<BalanceDepositController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult DepositBalance(int id)
        {
            var player = _players.FirstOrDefault(p => p.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        [HttpPost]
        public IActionResult DepositBalance(int id, decimal depositAmount)
        {
            var player = _players.FirstOrDefault(p => p.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            if (depositAmount <= 0)
            {
                ModelState.AddModelError("", "Částka musí být větší než 0.");
                return View(player);
            }

            player.Balance += depositAmount;
            _logger.LogInformation($"Hráč {player.Username} vložil {depositAmount}. Nový zůstatek: {player.Balance}");

            return RedirectToAction("DepositBalance", new { id = player.Id });
        }
    }
}