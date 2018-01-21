using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FinancialPortal.Models
{
    public class TransactionTypes
    {
        public int Id { get; set; }

        [Display(Name="Transaction Type")]
        public string Name { get; set; }
    }
}