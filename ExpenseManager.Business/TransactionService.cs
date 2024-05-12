using ExpenseManager.DataAccess.Data;
using ExpenseManager.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TransactionService
{
    private readonly ExpenseManagerDbContext _context;

    public TransactionService(ExpenseManagerDbContext context)
    {
        _context = context;
    }

    public async Task AddTransaction(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Transaction>, decimal)> GetUserTransactionsWithBalance(string userId, int pageIndex, int pageSize)
    {
        var transactionsQuery = _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.TransactionDate);

        var transactions = await transactionsQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Perform the sum operation in-memory
        var balance = transactionsQuery
            .AsEnumerable() // Switch to LINQ to Objects
            .Sum(t => t.Amount);

        return (transactions, balance);
    }

    // TODO move into CategoryService
    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        return await _context.Categories.ToListAsync();
    }
}
