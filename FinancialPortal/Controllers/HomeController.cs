using FinancialPortal.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Net;
using Microsoft.Owin.Security;
using System.Data.Entity;

namespace FinancialPortal.Controllers
{

    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
        {

            //var user = User.Identity.GetUserId();
            //var currentUser = db.Users.Find(user);
            //var house = db.Households.Find(currentUser.HouseholdId);
            //var houseBudget = db.Budgets.Where(b => b.HouseholdId == currentUser.HouseholdId);
            //var houseBudgetItems = houseBudget.SelectMany(b => b.BudgetItem);
            //var budgetCat = houseBudgetItems.Select(m => m.Category).Distinct().ToList();
            //var tod = System.DateTimeOffset.Now;

            //List<BudViewMod> budViewMod = new List<BudViewMod>();
            //var cat = db.Categories.Include(c => c.Name).Include(c => c.Id).Include(c => c.Transactions).Include(c => c.BudgetItems);

            //foreach (var item in budgetCat)
            //{
            //    var budView = new BudViewMod();
            //    var act = item.Transactions.Where(t => t.CreateDate.Year == tod.Year && t.CreateDate.Month == tod.Month)
            //                                            .Select(s => s.Amount)
            //                                            .DefaultIfEmpty().Sum();
            //    var bud = item.BudgetItems.Select(c => c.Amount)
            //                                            .DefaultIfEmpty()
            //                                            .Sum();

            //    act = (act < 0) ? act * -1 : act;
            //    bud = (bud < 0) ? bud * -1 : bud;

            //    var pct = (bud != 0) ? act / bud : 1;
            //    budView.Category = item.Name;
            //    budView.CatId = item.Id;
            //                          .Sum();

            //    budView.BudgetEst = bud;
            //    budView.ExpenseAct = act;
            //    budView.ExpPct = pct;
            //    budViewMod.Add(budView);
                
            //}
            //var budView = (from c in budgetCat
            //           where c.Id >= 7
            //           let aSum = (from t in c.Transactions
            //                       where t.CreateDate.Year == tod.Year && t.CreateDate.Month == tod.Month
            //                       select t.Amount).DefaultIfEmpty().Sum()
            //           let bSum = (from b in c.BudgetItems
            //                       select b.Amount).DefaultIfEmpty().Sum()
            //               let _ = bud.ExpenseAct = aSum
            //               let __ = bud.BudgetEst = bSum
            //               let ___ = bud.Category = c.Name
            //               select new
            //           {
            //               Name = c.Name,
            //               Actual = aSum * -1,
            //               Budgeted = bSum * -1
            //           }).ToArray();

            //return View(budViewMod);
            return View();
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        [Authorize]
        public ActionResult Expenses()
        {
            //ViewBag.Message = "Test page";
            //return View();

            var user = User.Identity.GetUserId();
            var currentUser = db.Users.Find(user);
            var house = db.Households.Find(currentUser.HouseholdId);
            var houseBudget = db.Budgets.Where(b => b.HouseholdId == currentUser.HouseholdId);
            var houseBudgetItems = houseBudget.SelectMany(b => b.BudgetItem);
            var budgetCat = houseBudgetItems.Select(m => m.Category).Distinct().ToList();
            var tod = System.DateTimeOffset.Now;

            List<BudViewMod> budViewMod = new List<BudViewMod>();
            var cat = db.Categories.Include(c => c.Name).Include(c => c.Id).Include(c => c.Transactions).Include(c => c.BudgetItems);

            foreach (var item in budgetCat)
            {
                var budView = new BudViewMod();
                var act = item.Transactions.Where(t => t.CreateDate.Year == tod.Year && t.CreateDate.Month == tod.Month)
                                                        .Select(s => s.Amount)
                                                        .DefaultIfEmpty().Sum();
                var bud = item.BudgetItems.Select(c => c.Amount)
                                                        .DefaultIfEmpty()
                                                        .Sum();
  
                // Reverse sign if amounts are negative
                act = (act < 0) ? act * -1 : act;
                bud = (bud < 0) ? bud * -1 : bud;

                var pct = (bud != 0) ? act / bud : 1;
                budView.Category = item.Name;
                budView.CatId = item.Id;
                //bud.BudgetEst = item.Transactions.Where(t => t.CreateDate.Year == tod.Year && t.CreateDate.Month == tod.Month)
                //                                        .Select(s => s.Amount)
                //                                        .DefaultIfEmpty().Sum();
                //bud.ExpenseAct = item.BudgetItems.Select(c => c.Amount)
                //                                        .DefaultIfEmpty()
                //                                        .Sum();

                budView.BudgetEst = bud;
                budView.ExpenseAct = act;
                budView.ExpPct = pct;
                budViewMod.Add(budView);

            }
            //var budView = (from c in budgetCat
            //           where c.Id >= 7
            //           let aSum = (from t in c.Transactions
            //                       where t.CreateDate.Year == tod.Year && t.CreateDate.Month == tod.Month
            //                       select t.Amount).DefaultIfEmpty().Sum()
            //           let bSum = (from b in c.BudgetItems
            //                       select b.Amount).DefaultIfEmpty().Sum()
            //               let _ = bud.ExpenseAct = aSum
            //               let __ = bud.BudgetEst = bSum
            //               let ___ = bud.Category = c.Name
            //               select new
            //           {
            //               Name = c.Name,
            //               Actual = aSum * -1,
            //               Budgeted = bSum * -1
            //           }).ToArray();

            return View(budViewMod);
        }

        //[HttpGet]
        //public ActionResult GetChart()
        //{
        //    var s = new[] {
        //                    new {label = "2008", value=20 },
        //                    new {label = "2009", value = 5 },
        //                    new {label = "2010", value = 7 },
        //                    new {label = "2011", value = 10 },
        //                    new {label = "2012", value = 20 }
        //                   };
        //    return Content(JsonConvert.SerializeObject(s), "application/json");
        //}

        [Authorize]
        public ActionResult GetChart()
        {
            var s = new[] { new { year= "2008", value= 20 },
                new { year= "2009", value= 5 },
                new { year= "2010", value= 7 },
                new { year= "2011", value= 10 },
                new { year= "2012", value= 20 }};

            var user = User.Identity.GetUserId();
            var currentUser = db.Users.Find(user);
            var house = db.Households.Find(currentUser.HouseholdId);

            //var house = db.Households.Find(User.Identity.GetHouseholdId<int>());
            var tod = System.DateTimeOffset.Now;
            decimal totalExpense = 0;
            decimal totalBudget = 0;

            if (currentUser.HouseholdId != null)
            {

                var totalAcc = (from a in house.Accounts
                            select a.Balance).DefaultIfEmpty().Sum();

            

                // Pull account info
                var houseAccts = db.Accounts.Where(p => p.HouseholdId == currentUser.HouseholdId);
                var houseTrans = houseAccts.SelectMany(t => t.Transactions);
                var houseType = houseTrans.Select(h => h.Category).Distinct().ToList();
                var totAcct = houseAccts.Select(u => u.Balance).DefaultIfEmpty().Sum();

                // Retrieve expense ifo
                var expense = houseAccts.SelectMany(t => t.Transactions).Where(t => t.TransactionTypeId == 2
                                                                                //&& t.CreateDate.Year == tod.Year  -- Hard coding below for demo
                                                                                //&& t.CreateDate.Month == tod.Month
                                                                                && t.Deleted == false
                                                                                && t.IsVoid == false
                                                                                && ((t.CreateDate.Year == 2016
                                                                                && t.CreateDate.Month >= 11)
                                                                                || t.CreateDate.Year >= 2017));
                var totExpense = expense.Select(b => b.Amount).DefaultIfEmpty().Sum();

                // Pull budget
                var houseBudget = db.Budgets.Where(b => b.HouseholdId == currentUser.HouseholdId);
                var houseBudgetItems = houseBudget.SelectMany(b => b.BudgetItem);
                var budgetCat = houseBudgetItems.Select(m => m.Category).Distinct().ToList();
                var totBudget = houseBudget.SelectMany(i => i.BudgetItem).Where(i => i.CategoryId >= 7).Select(a => a.Amount).DefaultIfEmpty().Sum();

                var bar = (from c in budgetCat
                                //where c.Id >=7
                            where c.Id >= 7
                            //let aSum = (from t in c.Transactions
                            //            where t.CreateDate.Year == tod.Year && t.CreateDate.Month == tod.Month
                            //            select t.Amount).DefaultIfEmpty().Sum()
                            //let bSum = (from b in c.BudgetItems
                            //           select b.Amount).DefaultIfEmpty().Sum()

                           let aSum = (from t in c.Transactions
                                       where (t.CreateDate.Year == 2016 && t.CreateDate.Month >= 11) || (t.CreateDate.Year >= 2017)
                                       select t.Amount).DefaultIfEmpty().Sum()
                           let bSum = (from b in c.BudgetItems
                                       select b.Amount).DefaultIfEmpty().Sum()

                           //let _ = totalExpense += aSum
                           //let __ = totalBudget += bSum

                           select new
                           {
                               Name = c.Name,
                               Actual = aSum * -1,
                               Budgeted = bSum * -1
                           }).ToArray();

                var donutExpense = (from c in houseType
                                    where c.Id >= 7
                                    let aSum = (from t in c.Transactions
                                                    //where t.CreateDate.Year == tod.Year && t.CreateDate.Month == tod.Month
                                                where (t.CreateDate.Year == 2016 && t.CreateDate.Month >= 11) || t.CreateDate.Year >= 2017
                                         select t.Amount).DefaultIfEmpty().Sum()
                             select new
                             {
                                 label = c.Name,
                                 value = aSum * -1
                             }).ToArray();

                var donutBudget = (from c in budgetCat
                            where c.Id >= 7
                            let bSum = (from t in c.BudgetItems
                                    select t.Amount).DefaultIfEmpty().Sum()
                                select new
                                {
                                    label = c.Name,
                                    value = bSum * -1
                                }).ToArray();

                var result = new
                {
                    totalAcc = totAcct,
                    totalExpense = totExpense * -1,
                    totalBudget = totBudget * -1,
                    //totalBudget = totalBudget,
                    //totalExpense = totalExpense,
                    bar = bar,
                    donutExpense = donutExpense,
                    donutBudget = donutBudget
                };

                return Content(JsonConvert.SerializeObject(result), "application/json");
             }

            return RedirectToAction("Join", "Invitations");
        }

    }
}