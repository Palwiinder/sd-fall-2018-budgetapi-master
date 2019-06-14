using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAPIBudget.Models
{
    public class CreateBankAccountBindingModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int HouseholdId { get; set; }
    }
}