using ExpenseManager.DataAccess.Data;
using ExpenseManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Business
{
    public class CategoryService
    {
        private readonly ExpenseManagerDbContext _context;

        public CategoryService(ExpenseManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetUserCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetUserCategories(string userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Category?> GetCategory(int categoryId, string userId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.UserId == userId);
        }

        public async Task SeedDefaultCategories(string userId)
        {
            var categories = new List<Category>
            {
                new Category { UserId = userId, Name = "Income" },
                new Category { UserId = userId, Name = "Groceries" },
                new Category { UserId = userId, Name = "Rent" },
                new Category { UserId = userId, Name = "Utilities" },
                new Category { UserId = userId, Name = "Transportation" },
                new Category { UserId = userId, Name = "Entertainment" },
                new Category { UserId = userId, Name = "Health" },
                new Category { UserId = userId, Name = "Insurance" },
                new Category { UserId = userId, Name = "Education" },
                new Category { UserId = userId, Name = "Withdrawal" },
                new Category { UserId = userId, Name = "Miscellaneous" }
            };

            foreach (var category in categories)
            {
                if (!_context.Categories.Any(c => c.Name == category.Name && c.UserId == userId))
                {
                    _context.Categories.Add(category);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddCategory(Category category)
        {
            _context.Categories.Add(category);
            var entriesWritten = await _context.SaveChangesAsync();
            return entriesWritten > 0;
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            var entitiesWritten = await _context.SaveChangesAsync();
            return entitiesWritten > 0;
        }

        public async Task<bool> RemoveCategory(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Transactions)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category is not null && !category.Transactions.Any())
            {
                _context.Remove(category);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
