using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIBudget.Models
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public int BankAccountId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool IsVoided { get; set; }
    }
}