using ExpenseManager.Business;
using ExpenseManager.DataAccess.Models;
using ExpenseManager.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

[Authorize]
public class TransactionController : Controller
{
    private readonly TransactionService _transactionService;
    private readonly CategoryService _categoryService;
    private readonly UserManager<IdentityUser> _userManager;
    private const int DashboardTransactionCount = 5;
    private const int PageSize = 10;

    public TransactionController(TransactionService transactionService, CategoryService categoryService, UserManager<IdentityUser> userManager)
    {
        _transactionService = transactionService;
        _categoryService = categoryService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(
        int pageIndex = 1,
        int pageSize = PageSize,
        int? categoryId = null,
        int? month = null,
        int? year = null,
        string description = null)
    {
        var userId = _userManager.GetUserId(User);
        var (transactions, totalCount) = await _transactionService.FilterUserTransactions(userId, pageIndex, pageSize, categoryId, month, year, description);
        var balance = await _transactionService.GetUserBalance(userId);

        ViewBag.Categories = await _categoryService.GetUserCategories(userId);

        var viewModel = new TransactionIndexViewModel
        {
            Transactions = transactions.ToList(),
            Balance = balance,
            CurrentPage = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount,
            FilterCategoryId = categoryId,
            FilterMonth = month,
            FilterYear = year,
            FilterDescription = description
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

    public async Task<IActionResult> Update(int transactionId)
    {
        var transaction = await _transactionService.GetTransaction(transactionId);
        if (transaction is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var viewModel = new TransactionViewModel
        {
            Id = transaction.TransactionId,
            Amount = transaction.Amount,
            TransactionDate = transaction.TransactionDate,
            Description = transaction.Description,
            CategoryId = transaction.CategoryId
        };

        var userId = _userManager.GetUserId(User);
        ViewBag.Categories = new SelectList(await _categoryService.GetUserCategories(userId), "CategoryId", "Name");
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(TransactionViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        if (ModelState.IsValid)
        {

            var transaction = new Transaction
            {
                UserId = userId,
                TransactionId = model.Id,
                Amount = model.Amount,
                TransactionDate = model.TransactionDate,
                Description = model.Description,
                CategoryId = model.CategoryId
            };
            await _transactionService.UpdateTransaction(transaction);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(await _categoryService.GetUserCategories(userId), "CategoryId", "Name");
        return View(model);
    }

    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User);
        var transactions = await _transactionService.GetLastNUserTransactions(userId, DashboardTransactionCount);
        var balance = await _transactionService.GetUserBalance(userId);

        var categorySummaries = transactions
        .GroupBy(t => t.Category.Name)
        .Select(g => new CategorySummary
        {
            CategoryName = g.Key,
            TotalAmount = g.Sum(t => t.Amount)
        })
        .ToList();

        var viewModel = new TransactionDashboardViewModel
        {
            Transactions = transactions,
            Balance = balance,
            CategorySummaries = categorySummaries
        };
        return View(viewModel);
    }

    public IActionResult ImportExport()
    {
        return View();
    }

    public async Task<IActionResult> ExportTransactions()
    {
        var userId = _userManager.GetUserId(User);
        var transactions = await _transactionService.GetUserTransactions(userId);
                    
        var exportTransactions = transactions.Select(t => new TransactionImportExportViewModel
        {
            Amount = t.Amount,
            TransactionDate = t.TransactionDate,
            Description = t.Description,
            CategoryId = t.CategoryId,
            CategoryName = t.Category.Name
        }).ToList();

        string json = await Task.Run(() => JsonConvert.SerializeObject(exportTransactions, Formatting.Indented));
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        return File(jsonBytes, "application/json", "transactions.json");
    }

    [HttpPost]
    public async Task<IActionResult> ImportTransactions(IFormFile transactionFile)
    {
        if (transactionFile == null || transactionFile.Length == 0)
        {
            ModelState.AddModelError("File", "Please upload a JSON file.");
            return View("ImportExport");
        }

        List<TransactionImportExportViewModel> transactions;
        try
        {
            using (var reader = new StreamReader(transactionFile.OpenReadStream()))
            {
                var fileContent = await reader.ReadToEndAsync();
                transactions = JsonConvert.DeserializeObject<List<TransactionImportExportViewModel>>(fileContent);
            }
        }
        catch (JsonException)
        {
            ModelState.AddModelError("File", "Invalid JSON format.");
            return View("ImportExport");
        }
        catch (Exception)
        {
            ModelState.AddModelError("File", "Error reading the file.");
            return View("ImportExport");
        }

        var userId = _userManager.GetUserId(User);
        var categories = await _categoryService.GetUserCategories(userId);

        var (transactionsIgnored, transactionsAdded) = await ProcessTransactions(transactions, userId, categories);

        TempData["TransactionAdded"] = transactionsAdded;
        TempData["TransactionIgnored"] = transactionsIgnored;
        return RedirectToAction("ImportExport");
    }

    private async Task<(int transactionsIgnored, int transactionsAdded)> ProcessTransactions(
    List<TransactionImportExportViewModel> transactions, string userId, IEnumerable<Category> categories)
    {
        var transactionsIgnored = 0;
        var transactionsAdded = 0;

        foreach (var transaction in transactions)
        {
            if (!ModelState.IsValid)
            {
                transactionsIgnored++;
                continue;
            }

            if (!categories.Any(c => c.CategoryId == transaction.CategoryId))
            {
                transactionsIgnored++;
                continue;
            }

            var newTransaction = new Transaction
            {
                UserId = userId,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Description = transaction.Description,
                CategoryId = transaction.CategoryId
            };

            var wasAdded = await _transactionService.AddTransaction(newTransaction);
            if (wasAdded)
            {
                transactionsAdded++;
            }
            else
            {
                transactionsIgnored++;
            }
        }

        return (transactionsIgnored, transactionsAdded);
    }
}
