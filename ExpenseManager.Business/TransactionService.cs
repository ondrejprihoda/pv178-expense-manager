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

    public async Task<bool> AddTransaction(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        var entitiesWritten = await _context.SaveChangesAsync();
        return entitiesWritten > 0;
    }

    public async Task<(IEnumerable<Transaction>, int)> FilterUserTransactions(
        string userId,
        int pageIndex,
        int pageSize,
        int? categoryId = null,
        int? month = null,
        int? year = null,
        string description = null)
    {
        var transactionsQuery = _context.Transactions
        .Include(t => t.Category)
        .Where(t => t.UserId == userId);

        if (categoryId.HasValue)
        {
            transactionsQuery = transactionsQuery.Where(t => t.CategoryId == categoryId);
        }

        if (month.HasValue)
        {
            transactionsQuery = transactionsQuery.Where(t => t.TransactionDate.Month == month);
        }

        if (year.HasValue)
        {
            transactionsQuery = transactionsQuery.Where(t => t.TransactionDate.Year == year);
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            transactionsQuery = transactionsQuery.Where(t => t.Description.Contains(description));
        }

        transactionsQuery = transactionsQuery.OrderByDescending(t => t.TransactionDate);

        var totalCount = await transactionsQuery.CountAsync();

        var transactions = await transactionsQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (transactions, totalCount);
    }

    public async Task<IEnumerable<Transaction>> GetUserTransactions(string userId)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetLastNUserTransactions(string userId, int transactionCount)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.TransactionDate)
            .Take(transactionCount)
            .ToListAsync();
    }

    public async Task<double> GetUserBalance(string userId)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .SumAsync(t => t.Amount);
    }

    public async Task<Transaction?> GetTransaction(int transactionId)
    {
        return await _context.Transactions.FindAsync(transactionId);
    }

    public async Task<bool> RemoveTransaction(int transactionId)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        if (transaction is not null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> UpdateTransaction(Transaction transaction)
    {
        var existingTransaction = await _context.Transactions.FindAsync(transaction.TransactionId);
        if (existingTransaction is not null)
        {
            existingTransaction.Amount = transaction.Amount;
            existingTransaction.TransactionDate = transaction.TransactionDate;
            existingTransaction.Description = transaction.Description;
            existingTransaction.CategoryId = transaction.CategoryId;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> SeedDefaultTransactions(string userId)
    {
        var categories = await _context.Categories
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var incomeCategory = categories.FirstOrDefault(c => c.Name == "Income").CategoryId;
        var groceriesCategory = categories.FirstOrDefault(c => c.Name == "Groceries").CategoryId;
        var rentCategory = categories.FirstOrDefault(c => c.Name == "Rent").CategoryId;
        var utilitiesCategory = categories.FirstOrDefault(c => c.Name == "Utilities").CategoryId;
        var transportationCategory = categories.FirstOrDefault(c => c.Name == "Transportation").CategoryId;
        var entertainmentCategory = categories.FirstOrDefault(c => c.Name == "Entertainment").CategoryId;
        var healthCategory = categories.FirstOrDefault(c => c.Name == "Health").CategoryId;
        var insuranceCategory = categories.FirstOrDefault(c => c.Name == "Insurance").CategoryId;
        var educationCategory = categories.FirstOrDefault(c => c.Name == "Education").CategoryId;
        var withdrawalCategory = categories.FirstOrDefault(c => c.Name == "Withdrawal").CategoryId;
        var miscellaneousCategory = categories.FirstOrDefault(c => c.Name == "Miscellaneous").CategoryId;

        var transactions = new List<Transaction>
        {
            // January Transactions
            new Transaction { UserId = userId, Amount = 50000, TransactionDate = new DateTime(2024, 1, 1), Description = "Salary", CategoryId = incomeCategory },
            new Transaction { UserId = userId, Amount = -3500, TransactionDate = new DateTime(2024, 1, 5), Description = "Grocery Shopping", CategoryId = groceriesCategory },
            new Transaction { UserId = userId, Amount = -15000, TransactionDate = new DateTime(2024, 1, 1), Description = "Monthly Rent", CategoryId = rentCategory },
            new Transaction { UserId = userId, Amount = -2500, TransactionDate = new DateTime(2024, 1, 10), Description = "Electricity Bill", CategoryId = utilitiesCategory },
            new Transaction { UserId = userId, Amount = -800, TransactionDate = new DateTime(2024, 1, 15), Description = "Bus Pass", CategoryId = transportationCategory },
            new Transaction { UserId = userId, Amount = -1200, TransactionDate = new DateTime(2024, 1, 20), Description = "Concert Tickets", CategoryId = entertainmentCategory },
            new Transaction { UserId = userId, Amount = -1500, TransactionDate = new DateTime(2024, 1, 25), Description = "Pharmacy", CategoryId = healthCategory },
            new Transaction { UserId = userId, Amount = -3000, TransactionDate = new DateTime(2024, 1, 3), Description = "Health Insurance", CategoryId = insuranceCategory },
            new Transaction { UserId = userId, Amount = -10000, TransactionDate = new DateTime(2024, 1, 12), Description = "Tuition Fee", CategoryId = educationCategory },
            new Transaction { UserId = userId, Amount = -5000, TransactionDate = new DateTime(2024, 1, 18), Description = "ATM Withdrawal", CategoryId = withdrawalCategory },
            new Transaction { UserId = userId, Amount = -1200, TransactionDate = new DateTime(2024, 1, 22), Description = "Miscellaneous Expenses", CategoryId = miscellaneousCategory },

            // February Transactions
            new Transaction { UserId = userId, Amount = 45000, TransactionDate = new DateTime(2024, 2, 1), Description = "Salary", CategoryId = incomeCategory },
            new Transaction { UserId = userId, Amount = -4000, TransactionDate = new DateTime(2024, 2, 5), Description = "Grocery Shopping", CategoryId = groceriesCategory },
            new Transaction { UserId = userId, Amount = -15000, TransactionDate = new DateTime(2024, 2, 1), Description = "Monthly Rent", CategoryId = rentCategory },
            new Transaction { UserId = userId, Amount = -2700, TransactionDate = new DateTime(2024, 2, 10), Description = "Water Bill", CategoryId = utilitiesCategory },
            new Transaction { UserId = userId, Amount = -900, TransactionDate = new DateTime(2024, 2, 15), Description = "Taxi", CategoryId = transportationCategory },
            new Transaction { UserId = userId, Amount = -1500, TransactionDate = new DateTime(2024, 2, 20), Description = "Movie Night", CategoryId = entertainmentCategory },
            new Transaction { UserId = userId, Amount = -1300, TransactionDate = new DateTime(2024, 2, 25), Description = "Doctor Appointment", CategoryId = healthCategory },
            new Transaction { UserId = userId, Amount = -3000, TransactionDate = new DateTime(2024, 2, 3), Description = "Car Insurance", CategoryId = insuranceCategory },
            new Transaction { UserId = userId, Amount = -10000, TransactionDate = new DateTime(2024, 2, 12), Description = "Course Materials", CategoryId = educationCategory },
            new Transaction { UserId = userId, Amount = -5000, TransactionDate = new DateTime(2024, 2, 18), Description = "ATM Withdrawal", CategoryId = withdrawalCategory },
            new Transaction { UserId = userId, Amount = -1500, TransactionDate = new DateTime(2024, 2, 22), Description = "Miscellaneous Expenses", CategoryId = miscellaneousCategory },

            // March Transactions
            new Transaction { UserId = userId, Amount = 30000, TransactionDate = new DateTime(2024, 3, 1), Description = "Salary", CategoryId = incomeCategory },
            new Transaction { UserId = userId, Amount = -3500, TransactionDate = new DateTime(2024, 3, 5), Description = "Grocery Shopping", CategoryId = groceriesCategory },
            new Transaction { UserId = userId, Amount = -15000, TransactionDate = new DateTime(2024, 3, 1), Description = "Monthly Rent", CategoryId = rentCategory },
            new Transaction { UserId = userId, Amount = -2500, TransactionDate = new DateTime(2024, 3, 10), Description = "Gas Bill", CategoryId = utilitiesCategory },
            new Transaction { UserId = userId, Amount = -800, TransactionDate = new DateTime(2024, 3, 15), Description = "Train Ticket", CategoryId = transportationCategory },
            new Transaction { UserId = userId, Amount = -1700, TransactionDate = new DateTime(2024, 3, 20), Description = "Theater Tickets", CategoryId = entertainmentCategory },
            new Transaction { UserId = userId, Amount = -1500, TransactionDate = new DateTime(2024, 3, 25), Description = "Pharmacy", CategoryId = healthCategory },
            new Transaction { UserId = userId, Amount = -3000, TransactionDate = new DateTime(2024, 3, 3), Description = "Health Insurance", CategoryId = insuranceCategory },
            new Transaction { UserId = userId, Amount = -10000, TransactionDate = new DateTime(2024, 3, 12), Description = "Tuition Fee", CategoryId = educationCategory },
            new Transaction { UserId = userId, Amount = -5000, TransactionDate = new DateTime(2024, 3, 18), Description = "ATM Withdrawal", CategoryId = withdrawalCategory },
            new Transaction { UserId = userId, Amount = -1200, TransactionDate = new DateTime(2024, 3, 22), Description = "Miscellaneous Expenses", CategoryId = miscellaneousCategory },

            // April Transactions
            new Transaction { UserId = userId, Amount = 60000, TransactionDate = new DateTime(2024, 4, 1), Description = "Salary", CategoryId = incomeCategory },
            new Transaction { UserId = userId, Amount = -4000, TransactionDate = new DateTime(2024, 4, 5), Description = "Grocery Shopping", CategoryId = groceriesCategory },
            new Transaction { UserId = userId, Amount = -15000, TransactionDate = new DateTime(2024, 4, 1), Description = "Monthly Rent", CategoryId = rentCategory },
            new Transaction { UserId = userId, Amount = -2700, TransactionDate = new DateTime(2024, 4, 10), Description = "Internet Bill", CategoryId = utilitiesCategory },
            new Transaction { UserId = userId, Amount = -900, TransactionDate = new DateTime(2024, 4, 15), Description = "Taxi", CategoryId = transportationCategory },
            new Transaction { UserId = userId, Amount = -2000, TransactionDate = new DateTime(2024, 4, 20), Description = "Amusement Park", CategoryId = entertainmentCategory },
            new Transaction { UserId = userId, Amount = -1300, TransactionDate = new DateTime(2024, 4, 25), Description = "Doctor Appointment", CategoryId = healthCategory },
            new Transaction { UserId = userId, Amount = -3000, TransactionDate = new DateTime(2024, 4, 3), Description = "Car Insurance", CategoryId = insuranceCategory },
            new Transaction { UserId = userId, Amount = -10000, TransactionDate = new DateTime(2024, 4, 12), Description = "Course Materials", CategoryId = educationCategory },
            new Transaction { UserId = userId, Amount = -5000, TransactionDate = new DateTime(2024, 4, 18), Description = "ATM Withdrawal", CategoryId = withdrawalCategory },
            new Transaction { UserId = userId, Amount = -1500, TransactionDate = new DateTime(2024, 4, 22), Description = "Miscellaneous Expenses", CategoryId = miscellaneousCategory },

            // May Transactions
            new Transaction { UserId = userId, Amount = 50000, TransactionDate = new DateTime(2024, 5, 1), Description = "Salary", CategoryId = incomeCategory },
            new Transaction { UserId = userId, Amount = -3500, TransactionDate = new DateTime(2024, 5, 5), Description = "Grocery Shopping", CategoryId = groceriesCategory },
            new Transaction { UserId = userId, Amount = -15000, TransactionDate = new DateTime(2024, 5, 1), Description = "Monthly Rent", CategoryId = rentCategory },
            new Transaction { UserId = userId, Amount = -2500, TransactionDate = new DateTime(2024, 5, 10), Description = "Electricity Bill", CategoryId = utilitiesCategory },
            new Transaction { UserId = userId, Amount = -800, TransactionDate = new DateTime(2024, 5, 15), Description = "Bus Pass", CategoryId = transportationCategory },
            new Transaction { UserId = userId, Amount = -1200, TransactionDate = new DateTime(2024, 5, 20), Description = "Concert Tickets", CategoryId = entertainmentCategory },
            new Transaction { UserId = userId, Amount = -1500, TransactionDate = new DateTime(2024, 5, 25), Description = "Pharmacy", CategoryId = healthCategory },
            new Transaction { UserId = userId, Amount = -3000, TransactionDate = new DateTime(2024, 5, 3), Description = "Health Insurance", CategoryId = insuranceCategory },
            new Transaction { UserId = userId, Amount = -10000, TransactionDate = new DateTime(2024, 5, 12), Description = "Tuition Fee", CategoryId = educationCategory },
            new Transaction { UserId = userId, Amount = -5000, TransactionDate = new DateTime(2024, 5, 18), Description = "ATM Withdrawal", CategoryId = withdrawalCategory },
            new Transaction { UserId = userId, Amount = -1200, TransactionDate = new DateTime(2024, 5, 22), Description = "Miscellaneous Expenses", CategoryId = miscellaneousCategory },
        };

        foreach (var transaction in transactions)
        {
            if (!_context.Transactions.Any(t => t.Description == transaction.Description && t.UserId == userId))
            {
                _context.Transactions.Add(transaction);
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAllUserTransactions(string userId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .ToListAsync();

        if (transactions.Any())
        {
            _context.Transactions.RemoveRange(transactions);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
