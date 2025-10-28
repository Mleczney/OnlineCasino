using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Data;
using OnlineCasino.Models;

namespace OnlineCasino.Controllers
{
    public class LoginController : Controller
    {
        private readonly CasinoContext _context;

        public LoginController(CasinoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            if (_context.Players.Any(p => p.Username == username))
            {
                ViewBag.Message = "Uživatel už existuje 😅";
                return View();
            }

            var player = new Player
            {
                Username = username,
                Password = password,
                Balance = 1000
            };

            _context.Players.Add(player);
            _context.SaveChanges();

            ViewBag.Message = "Registrace úspěšná! ✅ Můžeš se přihlásit.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var player = _context.Players.FirstOrDefault(p => p.Username == username && p.Password == password);
            if (player == null)
            {
                ViewBag.Message = "Špatné jméno nebo heslo 😭";
                return View();
            }

            HttpContext.Session.SetInt32("PlayerId", player.Id);
            HttpContext.Session.SetString("Username", player.Username);
            HttpContext.Session.SetDecimal("Balance", player.Balance);
            HttpContext.Session.SetString("Role", player.IsAdmin ? "Admin" : "Player");


            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

    public static class SessionExtensions
    {
        public static void SetDecimal(this ISession session, string key, decimal value)
        {
            session.SetString(key, value.ToString());
        }

        public static decimal GetDecimal(this ISession session, string key)
        {
            return decimal.TryParse(session.GetString(key), out var value) ? value : 0;
        }
    }
}
