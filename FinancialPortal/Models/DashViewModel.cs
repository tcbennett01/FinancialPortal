using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class DashViewModel
    {
        public int HouseId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal ReconciledBalance { get; set; }

        public int TransId { get; set; }
        public string TransType { get; set; }
        public DateTimeOffset TransDate { get; set; }
        public decimal TransAmount { get; set; }
        public int TransCategoryId { get; set; }
        public string Description { get; set; }

    }
}