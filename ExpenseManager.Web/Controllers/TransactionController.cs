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

    public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 2)
    {
        var userId = _userManager.GetUserId(User);
        var (transactions, totalCount) = await _transactionService.GetUserTransactions(userId, pageIndex, pageSize);
        var balance = await _transactionService.GetUserBalance(userId);

        var viewModel = new TransactionIndexViewModel
        {
            Transactions = transactions.ToList(),
            Balance = balance,
            CurrentPage = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount
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

    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User);
        var transactions = await _transactionService.GetLastNUserTransactions(userId, 5);
        var balance = await _transactionService.GetUserBalance(userId);
        var viewModel = new TransactionDashboardViewModel
        {
            Transactions = transactions,
            Balance = balance
        };
        return View(viewModel);
    }
}
