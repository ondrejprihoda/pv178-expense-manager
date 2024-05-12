using ExpenseManager.DataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManager.DataAccess.Data
{
    public class ExpenseManagerDbContext : IdentityDbContext<IdentityUser>
    {
        public ExpenseManagerDbContext(DbContextOptions<ExpenseManagerDbContext> options) : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; } // Adding DbSet for Category

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Food" },
                new Category { CategoryId = 2, Name = "Utilities" },
                new Category { CategoryId = 3, Name = "Entertainment" },
                new Category { CategoryId = 4, Name = "Deposit" },
                new Category { CategoryId = 5, Name = "Withdrawal" },
                new Category { CategoryId = 6, Name = "Transfer" }
            );
        }
    }
}
