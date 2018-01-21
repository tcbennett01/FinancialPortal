using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class BudgetTypes
    {
        public int Id { get; set; }

        [Display(Name = "Budget Type")]
        public string Name { get; set; }
    }
}