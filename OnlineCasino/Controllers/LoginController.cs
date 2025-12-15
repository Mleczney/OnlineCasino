using Microsoft.AspNetCore.Mvc;

namespace OnlineCasino.Controllers
{
    public class LoginController : Controller
    {
        public LoginController()
        {
        }

        [HttpGet]
        public IActionResult Register()
        {
            return RedirectToAction("Register", "Account");
        }

        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            return RedirectToAction("Register", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "Account");
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
