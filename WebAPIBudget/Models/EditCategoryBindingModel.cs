using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAPIBudget.Models
{
    public class EditCategoryBindingModel
    {
        [Required]
        public string Name { get; set; }
        public string Owner { get; set; }
        public bool IsOwner { get; set; }
        public string Description { get; set; }
    }
}