namespace ExpenseManager.Web.Models
{
    public class TransactionImportExportViewModel
    {
        public required double Amount { get; set; }
        public required DateTime TransactionDate { get; set; }
        public required string Description { get; set; }
        public required int CategoryId { get; set; }
        public required string CategoryName { get; set; }
    }
}
