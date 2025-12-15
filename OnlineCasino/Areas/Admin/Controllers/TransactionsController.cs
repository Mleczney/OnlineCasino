using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Application.Interfaces;

namespace OnlineCasino.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index()
        {
            var transactions = await _transactionService.GetAllAsync();
            return View(transactions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var transaction = await _transactionService.GetByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _transactionService.GetByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _transactionService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
