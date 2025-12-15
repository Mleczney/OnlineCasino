using Microsoft.AspNetCore.Mvc;

namespace OnlineCasino.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}
