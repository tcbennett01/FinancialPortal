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
    public class AccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accounts
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(userId);  //Intermittent timeout when combined w/prev step - testing separate
            var houseId = currentUser.HouseholdId;

            // Return accounts belonging to user's household != archived
            var accounts = db.Accounts.Include(a => a.Household).Where(a => a.HouseholdId == houseId && a.IsArchived == false);
            ViewBag.HouseName = db.Households.Find(houseId).Name;
            return View(accounts.ToList());
        }

        // GET: Archived Accounts
        public ActionResult Archived()
        {
            var userId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(userId);  //Intermittent timeout when combined w/prev step - testing separate
            var houseId = currentUser.HouseholdId;

            // Return accounts belonging to user's household & == archived
            var accounts = db.Accounts.Include(a => a.Household).Where(a => a.HouseholdId == houseId && a.IsArchived == true);

            ViewBag.HouseName = db.Households.Find(houseId).Name;
            return View(accounts.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accounts accounts = db.Accounts.Find(id);
            if (accounts == null)
            {
                return HttpNotFound();
            }
            return View(accounts);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            //var userId = User.Identity.GetUserId();
            //var currentUser = db.Users.Find(userId);
            //var houseId = Convert.ToInt32(currentUser.HouseholdId);

            //ViewBag.HouseName = db.Households.Find(houseId).Name;

            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HouseholdId,Name,Balance,ReconciledBalance")] Accounts accounts)
        {

            var userId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(userId);  
            var houseId = Convert.ToInt32(currentUser.HouseholdId);

            // Set initial balance and House Id
            accounts.HouseholdId = houseId;
            accounts.Balance = 0;
            accounts.ReconciledBalance = 0;
            accounts.IsArchived = false;
    
            if (ModelState.IsValid)
            {
                db.Accounts.Add(accounts);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", accounts.HouseholdId);
            return View(accounts);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accounts accounts = db.Accounts.Find(id);
            if (accounts == null)
            {
                return HttpNotFound();
            }
            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", accounts.HouseholdId);
            return View(accounts);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Name,Balance,ReconciledBalance, IsArchived")] Accounts accounts)
        {
            //accounts.HouseholdId

            if (ModelState.IsValid)
            {
                db.Entry(accounts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", accounts.HouseholdId);
            return View(accounts);
        }




        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accounts accounts = db.Accounts.Find(id);
            if (accounts == null)
            {
                return HttpNotFound();
            }
            return View(accounts);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Accounts accounts = db.Accounts.Find(id);
            accounts.IsArchived = true;
            //db.Accounts.Remove(accounts);
            db.Entry(accounts).State = EntityState.Modified;
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
    }
}
