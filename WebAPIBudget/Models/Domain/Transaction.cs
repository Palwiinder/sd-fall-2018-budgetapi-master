using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIBudget.Models.Domain
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool IsVoided { get; set; }

        public virtual Category Category { get; set; }
        public int CategoryId { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public int BankAccountId { get; set; }

        public virtual ApplicationUser Owner { get; set; }
        public string OwnerId { get; set; }

        public Transaction()
        {
            DateCreated = DateTime.Now;
            IsVoided = false;
        }
    }
}