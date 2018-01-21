using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPortal.Models;
using Microsoft.AspNet.Identity;

namespace FinancialPortal.Controllers
{
    [Authorize]
    public class BudgetItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetItems
        public ActionResult Index()
        {

            var user = User.Identity.GetUserId();
            var currentUser = db.Users.Find(user);
            var house = db.Households.Where(h => h.Id == currentUser.HouseholdId);

            var houseBudget = db.Budgets.Where(b => b.HouseholdId == currentUser.HouseholdId);
            var budgetItems = houseBudget.SelectMany(i => i.BudgetItem);
            return View(budgetItems.ToList());
        }

        // GET: BudgetItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItems budgetItems = db.BudgetItems.Find(id);
            if (budgetItems == null)
            {
                return HttpNotFound();
            }
            return View(budgetItems);
        }

        // GET: BudgetItems/Create
        public ActionResult Create()
        {
            var user = User.Identity.GetUserId();
            var currentUser = db.Users.Find(user);
            var houseBudget = db.Budgets.Where(b => b.HouseholdId == currentUser.HouseholdId);
            var houseBudgetId = db.Budgets.Where(b => b.HouseholdId == currentUser.HouseholdId).Select(i => i.Id).FirstOrDefault();

            var budgetItem = new BudgetItems();
            budgetItem.BudgetId = houseBudgetId;
            ViewBag.BudgetId = new SelectList(houseBudget, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", "Group", 1);
            return View(budgetItem);
        }

        // POST: BudgetItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryId,BudgetId,Amount")] BudgetItems budgetItems)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItems.BudgetId);
                ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItems.CategoryId);
                return View(budgetItems);
            }

            UpdateBudget(budgetItems);
            db.BudgetItems.Add(budgetItems);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: BudgetItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItems budgetItems = db.BudgetItems.Find(id);
            if (budgetItems == null)
            {
                return HttpNotFound();
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItems.BudgetId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItems.CategoryId);
            ViewBag.BudgetName = db.Budgets.Find(budgetItems.BudgetId).Name;
            return View(budgetItems);
        }

        // POST: BudgetItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryId,BudgetId,Amount")] BudgetItems budgetItems)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItems.BudgetId);
                ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItems.CategoryId);
                return View(budgetItems);
            }

            // Reverse last budget item and update new
            ReverseBudgetItem(budgetItems);
            var amount = UpdateBudget(budgetItems);

            var oldbudgetItems = db.BudgetItems.Find(budgetItems.Id);
            oldbudgetItems.CategoryId = budgetItems.CategoryId;
            oldbudgetItems.Amount = amount;

            db.Entry(oldbudgetItems).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: BudgetItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItems budgetItems = db.BudgetItems.Find(id);
            if (budgetItems == null)
            {
                return HttpNotFound();
            }
            return View(budgetItems);
        }

        // POST: BudgetItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetItems budgetItems = db.BudgetItems.Find(id);
            ReverseBudgetItem(budgetItems);
            db.BudgetItems.Remove(budgetItems);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Update Budget Catetories
        public decimal UpdateBudget(BudgetItems budgetItem)
        {
            var currentBudget = db.Budgets.Find(budgetItem.BudgetId);
           
            // Check amount sign and reverse if necessary
            if ((budgetItem.CategoryId < 7 && budgetItem.Amount < 0)
                || (budgetItem.CategoryId >= 7 && budgetItem.Amount > 0))
            {
                budgetItem.Amount *= -1;
            }

            if (budgetItem.CategoryId < 7)
            {
                currentBudget.Income += budgetItem.Amount;
            }
            else if (budgetItem.CategoryId >= 7)
            {
                currentBudget.Expense += budgetItem.Amount;
            }

            db.Entry(currentBudget).State = EntityState.Modified;
            db.SaveChanges();
            return(budgetItem.Amount);
        }

        //Reverse Expense
        public void ReverseBudgetItem(BudgetItems budgetItem)
        {
            var oldBudget = db.Budgets.Find(budgetItem.BudgetId);
            var oldBudgetItem = db.BudgetItems.Find(budgetItem.Id);

            if (oldBudgetItem.CategoryId < 7)
            {
                oldBudget.Income -= oldBudgetItem.Amount;
            }
            else
            {
                oldBudget.Expense -= oldBudgetItem.Amount;
            }

            db.Entry(oldBudget).State = EntityState.Modified;
            db.SaveChanges();
            return;
        }
    }
}
