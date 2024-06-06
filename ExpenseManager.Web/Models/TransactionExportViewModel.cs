namespace ExpenseManager.Web.Models
{
    public class TransactionExportViewModel
    {
        public required int TransactionId { get; set; }
        public required double Amount { get; set; }
        public required DateTime TransactionDate { get; set; }
        public required string Description { get; set; }
        public required string CategoryName { get; set; }
    }

}
