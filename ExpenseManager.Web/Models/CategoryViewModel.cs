namespace ExpenseManager.Web.Models
{
    public class CategoryViewModel
    {
        public required int CategoryId { get; set; }
        public required string UserId { get; set; }
        public required string Name { get; set; }
    }
}
