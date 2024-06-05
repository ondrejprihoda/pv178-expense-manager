using Microsoft.AspNetCore.Identity;

namespace ExpenseManager.DataAccess.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public string UserId { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } // Navigation property to Category
        public virtual IdentityUser User { get; set; } // Assuming use of default Identity user
    }
}
