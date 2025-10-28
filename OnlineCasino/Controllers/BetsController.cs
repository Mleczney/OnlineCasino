using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineCasino.Data;
using OnlineCasino.Models;

namespace OnlineCasino.Controllers
{
    public class BetsController : Controller
    {
        private readonly CasinoContext _context;

        public BetsController(CasinoContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var casinoContext = _context.Bets.Include(b => b.Game).Include(b => b.Player);
            return View(await casinoContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bet = await _context.Bets
                .Include(b => b.Game)
                .Include(b => b.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bet == null)
            {
                return NotFound();
            }

            return View(bet);
        }

        public IActionResult Create()
        {
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Id");
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PlayerId,GameId,Amount")] Bet bet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Id", bet.GameId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id", bet.PlayerId);
            return View(bet);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bet = await _context.Bets.FindAsync(id);
            if (bet == null)
            {
                return NotFound();
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Id", bet.GameId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id", bet.PlayerId);
            return View(bet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlayerId,GameId,Amount")] Bet bet)
        {
            if (id != bet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BetExists(bet.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Id", bet.GameId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id", bet.PlayerId);
            return View(bet);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bet = await _context.Bets
                .Include(b => b.Game)
                .Include(b => b.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var bet = await _context.Bets.FindAsync(id);
            if (bet != null)
            {
                _context.Bets.Remove(bet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BetExists(int id)
        {
            return _context.Bets.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult Play()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Play(decimal amount, int guess)
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (amount <= 0)
            {
                ViewBag.Message = "Zadej kladnou částku 😅";
                return View();
            }

            var player = await _context.Players.FindAsync(playerId.Value);
            if (player == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (player.Balance < amount)
            {
                ViewBag.Message = "Nemáš dostatek kreditu 💀";
                return View();
            }

            var random = new Random();
            int rolled = random.Next(1, 6);
            bool win = rolled == guess;

            decimal result = 0;
            if (win)
            {
                result = amount * 2;
                player.Balance += result;
            }
            else
            {
                player.Balance -= amount;
            }

            var bet = new Bet
            {
                PlayerId = player.Id,
                GameId = 1,
                Amount = amount
            };

            _context.Bets.Add(bet);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetDecimal("Balance", player.Balance);

            ViewBag.Guess = guess;
            ViewBag.Rolled = rolled;
            ViewBag.Win = win;
            ViewBag.Amount = amount;
            ViewBag.Result = result;
            ViewBag.NewBalance = player.Balance;

            return View("Result");
        }


    }
}
