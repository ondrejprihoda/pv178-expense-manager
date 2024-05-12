using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models
{
    public class TransactionViewModel
    {
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }
}
