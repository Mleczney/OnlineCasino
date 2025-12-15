using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Application.Interfaces;

namespace OnlineCasino.Controllers
{
    [Authorize]
    public class SlotMachineController : Controller
    {
        private readonly IPlayerService _playerService;
        private readonly IGameService _gameService;

        public SlotMachineController(IPlayerService playerService, IGameService gameService)
        {
            _playerService = playerService;
            _gameService = gameService;
        }

        [HttpGet]
        public IActionResult Play()
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                TempData["Error"] = "Musíte být přihlášeni jako hráč, abyste mohli hrát.";
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Play(decimal betAmount)
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (betAmount < 10 || betAmount > 1000)
            {
                ModelState.AddModelError("", "Sázka musí být mezi 10 Kč a 1000 Kč");
                return View();
            }

            try
            {
                var player = await _playerService.GetByIdAsync(playerId.Value);
                if (player == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (player.Balance < betAmount)
                {
                    ModelState.AddModelError("", "Nemáte dostatek kreditu");
                    return View();
                }

                // Generate random symbols for 3 reels
                var random = new Random();
                string[] symbols = { "🍒", "🍋", "🍊", "🍇", "💎", "7️⃣", "🔔" };
                
                var reel1 = symbols[random.Next(symbols.Length)];
                var reel2 = symbols[random.Next(symbols.Length)];
                var reel3 = symbols[random.Next(symbols.Length)];

                // Check for win
                decimal winAmount = 0;
                bool isWin = false;
                string message = "";

                if (reel1 == reel2 && reel2 == reel3)
                {
                    // Three of a kind
                    if (reel1 == "7️⃣")
                    {
                        winAmount = betAmount * 10; // Jackpot!
                        message = "🎰 JACKPOT! Vyhráli jste 10x sázku!";
                    }
                    else if (reel1 == "💎")
                    {
                        winAmount = betAmount * 5;
                        message = "💎 Skvělé! Vyhráli jste 5x sázku!";
                    }
                    else
                    {
                        winAmount = betAmount * 3;
                        message = "🎉 Výhra! 3x sázka!";
                    }
                    isWin = true;
                }
                else if (reel1 == reel2 || reel2 == reel3 || reel1 == reel3)
                {
                    // Two of a kind
                    winAmount = betAmount * 1.5m;
                    message = "✨ Malá výhra! 1.5x sázka!";
                    isWin = true;
                }
                else
                {
                    // No win
                    message = "💀 Žádná výhra, zkuste to znovu!";
                }

                // Update player balance
                if (isWin)
                {
                    await _playerService.DepositAsync(playerId.Value, winAmount - betAmount);
                }
                else
                {
                    await _playerService.WithdrawAsync(playerId.Value, betAmount);
                }

                // Update session balance
                var updatedPlayer = await _playerService.GetByIdAsync(playerId.Value);
                HttpContext.Session.SetString("Balance", updatedPlayer.Balance.ToString());

                ViewBag.Reel1 = reel1;
                ViewBag.Reel2 = reel2;
                ViewBag.Reel3 = reel3;
                ViewBag.IsWin = isWin;
                ViewBag.WinAmount = winAmount;
                ViewBag.BetAmount = betAmount;
                ViewBag.Message = message;
                ViewBag.NewBalance = updatedPlayer.Balance;

                return View("Result");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
    }
}
