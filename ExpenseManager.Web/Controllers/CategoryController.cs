using ExpenseManager.Business;
using ExpenseManager.DataAccess.Models;
using ExpenseManager.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManager.Web.Controllers;

[Authorize]
public class CategoryController : Controller
{
    private readonly CategoryService _categoryService;
    private readonly UserManager<IdentityUser> _userManager;

    public CategoryController(CategoryService categoryService, UserManager<IdentityUser> userManager)
    {
        _categoryService = categoryService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var categories = await _categoryService.GetUserCategories(userId);

        var viewModel = new CategoryIndexViewModel
        {
            Categories = categories
        };

        return View(viewModel);
    }

    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(CategoryAddViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        if (ModelState.IsValid && userId is not null)
        {
            var category = new Category
            {
                UserId = userId,
                Name = model.Name
            };

            await _categoryService.AddCategory(category);
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public async Task<IActionResult> Remove(int categoryId)
    {
        var wasRemoved = await _categoryService.RemoveCategory(categoryId);
        if (!wasRemoved)
        {
            TempData["ResultMessage"] = "Couldn't remove category. There are associated transactions.";
        }
        return RedirectToAction("Index");
    }
}
