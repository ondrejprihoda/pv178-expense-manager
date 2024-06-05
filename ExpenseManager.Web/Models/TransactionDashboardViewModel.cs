using ExpenseManager.DataAccess.Models;

namespace ExpenseManager.Web.Models
{
    public class TransactionDashboardViewModel
    {
        public required IEnumerable<Transaction> Transactions { get; set; }
        public required double Balance { get; set; }
    }
}
