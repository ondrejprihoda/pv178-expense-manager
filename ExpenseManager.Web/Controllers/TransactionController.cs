using ExpenseManager.DataAccess.Models;
using ExpenseManager.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

[Authorize]
public class TransactionController : Controller
{
    private readonly TransactionService _transactionService;
    private readonly UserManager<IdentityUser> _userManager;

    public TransactionController(TransactionService transactionService, UserManager<IdentityUser> userManager)
    {
        _transactionService = transactionService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
    {
        var userId = _userManager.GetUserId(User);
        var (transactions, balance) = await _transactionService.GetUserTransactionsWithBalance(userId, pageIndex, pageSize);
        var viewModel = new TransactionIndexViewModel
        {
            Transactions = transactions,
            Balance = balance,
            CurrentPage = pageIndex,
            PageSize = pageSize
        };
        return View(viewModel);
    }

    public async Task<IActionResult> Add()
    {
        ViewBag.Categories = new SelectList(await _transactionService.GetAllCategories(), "CategoryId", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(TransactionViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            var transaction = new Transaction
            {
                UserId = userId,
                Amount = model.Amount,
                TransactionDate = model.TransactionDate,
                Description = model.Description,
                CategoryId = model.CategoryId
            };
            await _transactionService.AddTransaction(transaction);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }
}
