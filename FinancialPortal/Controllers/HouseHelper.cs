using FinancialPortal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace FinancialPortal.Controllers
{
    public static class HouseHelper
    {

        // Join household
        //public void JoinHouse(int houseId, string userId)
        //{
        //    var house = db.Households.Find(houseId);
        //    var user = db.Users.Find(userId);
        //    house.Users.Add(user);
        //    db.SaveChanges();
        //}

        private static ApplicationDbContext db = new ApplicationDbContext();
        public static int GetHouseholdId(string userId)
        {
            return (int)db.Users.Find(userId).HouseholdId;
        }



    }
}