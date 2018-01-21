using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public static class Help
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static int GetHouseholdId(this string userId)
        {
            return (int)db.Users.Find(userId).HouseholdId;
        }
    }
}