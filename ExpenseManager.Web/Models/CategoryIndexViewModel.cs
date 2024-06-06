namespace ExpenseManager.Web.Models
{
    public class CategoryIndexViewModel
    {
        public required IEnumerable<DataAccess.Models.Category> Categories { get; set; }
    }
}
