using ExpenseManager.Business;
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
    private readonly CategoryService _categoryService;
    private readonly UserManager<IdentityUser> _userManager;

    public TransactionController(TransactionService transactionService, CategoryService categoryService, UserManager<IdentityUser> userManager)
    {
        _transactionService = transactionService;
        _categoryService = categoryService;
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
        var userId = _userManager.GetUserId(User);
        ViewBag.Categories = new SelectList(await _categoryService.GetUserCategories(userId), "CategoryId", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(TransactionViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        if (ModelState.IsValid)
        {
            
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

        ViewBag.Categories = new SelectList(await _categoryService.GetUserCategories(userId), "CategoryId", "Name");
        return View(model);
    }

    public async Task<IActionResult> Remove(int transactionId)
    {
        await _transactionService.RemoveTransaction(transactionId);
        string referringUrl = Request.Headers["Referer"].ToString();
        return Redirect(referringUrl);
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
