using ExpenseManager.DataAccess.Models;

namespace ExpenseManager.Web.Models
{
    public class TransactionDashboardViewModel
    {
        public required IEnumerable<Transaction> Transactions { get; set; }
        public required double Balance { get; set; }
        public required List<CategorySummary> CategorySummaries { get; set; }
    }

    public class CategorySummary
    {
        public required string CategoryName { get; set; }
        public required double TotalAmount { get; set; }
    }
}
