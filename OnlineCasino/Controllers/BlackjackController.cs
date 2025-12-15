using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Application.Interfaces;

namespace OnlineCasino.Controllers
{
    [Authorize]
    public class BlackjackController : Controller
    {
        private readonly IPlayerService _playerService;

        public BlackjackController(IPlayerService playerService)
        {
            _playerService = playerService;
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

            // Clear any existing game state
            HttpContext.Session.Remove("BJ_PlayerCards");
            HttpContext.Session.Remove("BJ_DealerCards");
            HttpContext.Session.Remove("BJ_Deck");
            HttpContext.Session.Remove("BJ_BetAmount");
            HttpContext.Session.Remove("BJ_GameOver");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartGame(decimal betAmount)
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (betAmount < 10 || betAmount > 1000)
            {
                ModelState.AddModelError("", "Sázka musí být mezi 10 Kč a 1000 Kč");
                return View("Play");
            }

            var player = await _playerService.GetByIdAsync(playerId.Value);
            if (player == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (player.Balance < betAmount)
            {
                ModelState.AddModelError("", "Nemáte dostatek kreditu");
                return View("Play");
            }

            // Deduct bet from player balance
            await _playerService.WithdrawAsync(playerId.Value, betAmount);
            var updatedPlayer = await _playerService.GetByIdAsync(playerId.Value);
            HttpContext.Session.SetString("Balance", updatedPlayer.Balance.ToString());

            // Initialize deck
            var deck = CreateDeck();
            Shuffle(deck);

            // Deal initial cards
            var playerCards = new List<string> { DrawCard(deck), DrawCard(deck) };
            var dealerCards = new List<string> { DrawCard(deck), DrawCard(deck) };

            // Store game state in session
            HttpContext.Session.SetString("BJ_PlayerCards", string.Join(",", playerCards));
            HttpContext.Session.SetString("BJ_DealerCards", string.Join(",", dealerCards));
            HttpContext.Session.SetString("BJ_Deck", string.Join(",", deck));
            HttpContext.Session.SetString("BJ_BetAmount", betAmount.ToString());
            HttpContext.Session.SetString("BJ_GameOver", "false");

            return RedirectToAction("Game");
        }

        [HttpGet]
        public IActionResult Game()
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var playerCardsStr = HttpContext.Session.GetString("BJ_PlayerCards");
            if (string.IsNullOrEmpty(playerCardsStr))
            {
                return RedirectToAction("Play");
            }

            var playerCards = playerCardsStr.Split(',').ToList();
            var dealerCards = HttpContext.Session.GetString("BJ_DealerCards")?.Split(',').ToList() ?? new List<string>();
            var gameOver = HttpContext.Session.GetString("BJ_GameOver") == "true";

            ViewBag.PlayerCards = playerCards;
            ViewBag.DealerCards = dealerCards;
            ViewBag.PlayerScore = CalculateScore(playerCards);
            ViewBag.DealerScore = CalculateScore(dealerCards);
            ViewBag.BetAmount = decimal.Parse(HttpContext.Session.GetString("BJ_BetAmount") ?? "0");
            ViewBag.GameOver = gameOver;
            ViewBag.ShowDealerFirstCardOnly = !gameOver;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Hit()
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var playerCardsStr = HttpContext.Session.GetString("BJ_PlayerCards");
            var deckStr = HttpContext.Session.GetString("BJ_Deck");

            if (string.IsNullOrEmpty(playerCardsStr) || string.IsNullOrEmpty(deckStr))
            {
                return RedirectToAction("Play");
            }

            var playerCards = playerCardsStr.Split(',').ToList();
            var deck = deckStr.Split(',').ToList();

            // Draw a card
            playerCards.Add(DrawCard(deck));

            // Update session
            HttpContext.Session.SetString("BJ_PlayerCards", string.Join(",", playerCards));
            HttpContext.Session.SetString("BJ_Deck", string.Join(",", deck));

            // Check if bust
            if (CalculateScore(playerCards) > 21)
            {
                HttpContext.Session.SetString("BJ_GameOver", "true");
                return RedirectToAction("Result");
            }

            return RedirectToAction("Game");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Stand()
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var dealerCardsStr = HttpContext.Session.GetString("BJ_DealerCards");
            var deckStr = HttpContext.Session.GetString("BJ_Deck");

            if (string.IsNullOrEmpty(dealerCardsStr) || string.IsNullOrEmpty(deckStr))
            {
                return RedirectToAction("Play");
            }

            var dealerCards = dealerCardsStr.Split(',').ToList();
            var deck = deckStr.Split(',').ToList();

            // Dealer draws until 17 or higher
            while (CalculateScore(dealerCards) < 17)
            {
                dealerCards.Add(DrawCard(deck));
            }

            // Update session
            HttpContext.Session.SetString("BJ_DealerCards", string.Join(",", dealerCards));
            HttpContext.Session.SetString("BJ_Deck", string.Join(",", deck));
            HttpContext.Session.SetString("BJ_GameOver", "true");

            return RedirectToAction("Result");
        }

        [HttpGet]
        public async Task<IActionResult> Result()
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var playerCardsStr = HttpContext.Session.GetString("BJ_PlayerCards");
            var dealerCardsStr = HttpContext.Session.GetString("BJ_DealerCards");
            var betAmountStr = HttpContext.Session.GetString("BJ_BetAmount");

            if (string.IsNullOrEmpty(playerCardsStr) || string.IsNullOrEmpty(dealerCardsStr) || string.IsNullOrEmpty(betAmountStr))
            {
                return RedirectToAction("Play");
            }

            var playerCards = playerCardsStr.Split(',').ToList();
            var dealerCards = dealerCardsStr.Split(',').ToList();
            var betAmount = decimal.Parse(betAmountStr);

            var playerScore = CalculateScore(playerCards);
            var dealerScore = CalculateScore(dealerCards);

            string result;
            decimal winAmount = 0;
            bool isWin = false;

            if (playerScore > 21)
            {
                result = "💀 PROHRA! Překročili jste 21.";
            }
            else if (dealerScore > 21)
            {
                result = "🎉 VÝHRA! Dealer překročil 21.";
                winAmount = betAmount * 2;
                isWin = true;
            }
            else if (playerScore > dealerScore)
            {
                result = "🎉 VÝHRA! Máte více bodů než dealer.";
                winAmount = betAmount * 2;
                isWin = true;
            }
            else if (playerScore == dealerScore)
            {
                result = "🤝 REMÍZA! Stejný počet bodů.";
                winAmount = betAmount;
                isWin = true;
            }
            else
            {
                result = "💀 PROHRA! Dealer má více bodů.";
            }

            // Update player balance if win or tie
            if (isWin)
            {
                await _playerService.DepositAsync(playerId.Value, winAmount);
                var updatedPlayer = await _playerService.GetByIdAsync(playerId.Value);
                HttpContext.Session.SetString("Balance", updatedPlayer.Balance.ToString());
            }

            ViewBag.PlayerCards = playerCards;
            ViewBag.DealerCards = dealerCards;
            ViewBag.PlayerScore = playerScore;
            ViewBag.DealerScore = dealerScore;
            ViewBag.Result = result;
            ViewBag.BetAmount = betAmount;
            ViewBag.WinAmount = winAmount;
            ViewBag.IsWin = isWin;

            return View();
        }

        // Helper methods
        private List<string> CreateDeck()
        {
            var suits = new[] { "♠", "♥", "♦", "♣" };
            var ranks = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            var deck = new List<string>();

            foreach (var suit in suits)
            {
                foreach (var rank in ranks)
                {
                    deck.Add($"{rank}{suit}");
                }
            }

            return deck;
        }

        private void Shuffle(List<string> deck)
        {
            var random = Random.Shared;
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                var temp = deck[k];
                deck[k] = deck[n];
                deck[n] = temp;
            }
        }

        private string DrawCard(List<string> deck)
        {
            var card = deck[0];
            deck.RemoveAt(0);
            return card;
        }

        private int CalculateScore(List<string> cards)
        {
            int score = 0;
            int aces = 0;

            foreach (var card in cards)
            {
                // Extract rank by removing the last character (suit symbol)
                // Handle multi-byte unicode characters properly
                var rank = card[..^1];

                if (rank == "A")
                {
                    aces++;
                    score += 11;
                }
                else if (rank == "J" || rank == "Q" || rank == "K")
                {
                    score += 10;
                }
                else if (int.TryParse(rank, out int cardValue))
                {
                    score += cardValue;
                }
            }

            // Adjust for aces
            while (score > 21 && aces > 0)
            {
                score -= 10;
                aces--;
            }

            return score;
        }
    }
}
