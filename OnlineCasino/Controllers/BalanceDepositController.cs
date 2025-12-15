using Microsoft.AspNetCore.Mvc;

namespace OnlineCasino.Controllers
{
    public class BalanceDepositController : Controller
    {
        [HttpGet]
        public IActionResult DepositBalance()
        {
            return RedirectToAction("Deposit", "Account");
        }

        [HttpPost]
        public IActionResult DepositBalance(decimal depositAmount)
        {
            return RedirectToAction("Deposit", "Account");
        }
    }
}
