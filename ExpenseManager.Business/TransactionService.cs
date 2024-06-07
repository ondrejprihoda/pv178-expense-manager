﻿using ExpenseManager.DataAccess.Data;
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
}
