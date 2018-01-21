using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class BudgetItems
    {

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int BudgetId { get; set; }

        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        // -- Navigation properties -- //
        public virtual Categories Category { get; set; }
        public virtual Budgets Budget { get; set; }
    }
}