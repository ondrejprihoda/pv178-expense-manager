using ExpenseManager.DataAccess.Models;

namespace ExpenseManager.Web.Models
{
    public class TransactionIndexViewModel
    {
        public required IEnumerable<Transaction> Transactions { get; set; }
        public required double Balance { get; set; }
        public required int CurrentPage { get; set; }
        public required int PageSize { get; set; }
        public required int TotalCount { get; set; }

        public int? FilterCategoryId { get; set; }
        public int? FilterMonth { get; set; }
        public int? FilterYear { get; set; }
        public string FilterDescription { get; set; }
    }
}
