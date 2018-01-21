using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class Accounts
    {
        public Accounts()
        {
            Transactions = new HashSet<Transactions>();
        }

        public int Id { get; set; }
        public int HouseholdId {get; set;}
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [DataType(DataType.Currency)]
        public decimal ReconciledBalance { get; set; }
        //public bool IsReconciled { get; set; }

        [Display(Name="Archived")]
        public bool IsArchived { get; set; }

        //-- Navigation Properties --//
        public virtual ICollection<Transactions>Transactions { get; set; }
        public virtual Households Household { get; set; }
    }

}