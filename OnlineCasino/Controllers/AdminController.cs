using Microsoft.AspNetCore.Mvc;

namespace OnlineCasino.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized(); // nebo RedirectToAction("Index", "Home")

            return View();
        }
    }
}
