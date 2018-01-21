using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace FinancialPortal.Models
{
    public class Budgets
    {
        public Budgets()
        {
            BudgetItem = new HashSet<BudgetItems>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int HouseholdId { get; set; }

        [DataType(DataType.Currency)]
        public decimal Income { get; set; }

        [DataType(DataType.Currency)]
        public decimal Expense { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTimeOffset Created { get; set; }

        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        //public DateTimeOffset? Modified { get; set; }
        
        //-- Navigation Properties --//
        public ICollection<BudgetItems>BudgetItem { get; set; }
        public virtual Households Household { get; set; }
    }

    public class BudViewMod
    {
        public string Category { get; set; }
        public int CatId { get; set; }
        public decimal ExpenseAct { get; set; }
        public decimal BudgetEst { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        public decimal ExpPct { get; set; }
    }
}