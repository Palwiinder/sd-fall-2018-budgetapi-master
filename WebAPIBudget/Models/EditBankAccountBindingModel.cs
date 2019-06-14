using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAPIBudget.Models
{
    public class EditBankAccountBindingModel
    {
        [Required]
        public string Name { get; set; }
        public int HouseHoldId { get; set; }
        public string Description { get; set; }
    }
}