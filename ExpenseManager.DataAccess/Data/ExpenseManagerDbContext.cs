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
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // seed initial data here:
        }
    }
}
