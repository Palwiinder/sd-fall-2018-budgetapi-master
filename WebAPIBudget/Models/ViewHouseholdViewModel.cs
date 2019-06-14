using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIBudget.Models
{
    public class ViewHouseholdViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfUsers { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public bool IsOwner { get; set; }
        public string Description { get; set; }
    }
}