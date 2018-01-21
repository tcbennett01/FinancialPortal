using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class DashboardViewModel
    {
        public Accounts Accounts { get; set; }
        public Transactions Transactions { get; set; }
        public Households Households { get; set; }
    }
}