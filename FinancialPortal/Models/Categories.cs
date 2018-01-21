using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class Categories
    {
        public Categories()
        {
            Transactions = new HashSet<Transactions>();
            BudgetItems = new HashSet<BudgetItems>();
            Households = new HashSet<Households>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }

        //-- Navigation Properties --//
        public virtual ICollection<Transactions> Transactions { get; set; }
        public virtual ICollection<BudgetItems> BudgetItems { get; set; }
        public virtual ICollection<Households> Households { get; set; }
    }
}