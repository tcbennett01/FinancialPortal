using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class Households
    {
        public Households()
        {
            Accounts = new HashSet<Accounts>();
            Budgets = new HashSet<Budgets>();
            Users = new HashSet<ApplicationUser>();
            Invitations = new HashSet<Invitations>();
            Categories = new HashSet<Categories>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        // -- Navigation properties -- //
        public virtual ICollection<Accounts>Accounts { get; set; }
        public virtual ICollection<Budgets>Budgets { get; set; }
        public virtual ICollection<ApplicationUser>Users { get; set; }
        public virtual ICollection<Invitations>Invitations { get; set; }
        public virtual ICollection<Categories>Categories { get; set; }
    }
}