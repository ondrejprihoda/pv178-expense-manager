using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models
{
    public class TransactionViewModel
    {
        public required int Id { get; set; }
        public required double Amount { get; set; }
        public required DateTime TransactionDate { get; set; }
        public required string Description { get; set; }
        public required int CategoryId { get; set; }
    }
}
