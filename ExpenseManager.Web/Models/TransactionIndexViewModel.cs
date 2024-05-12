using ExpenseManager.DataAccess.Models;

namespace ExpenseManager.Web.Models
{
    public class TransactionIndexViewModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public decimal Balance { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
