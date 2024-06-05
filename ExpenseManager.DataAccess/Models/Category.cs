namespace ExpenseManager.DataAccess.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
