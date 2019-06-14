using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAPIBudget.Models
{
    public class InviteBindingModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}