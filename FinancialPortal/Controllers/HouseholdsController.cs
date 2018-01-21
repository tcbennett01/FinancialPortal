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
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinancialPortal.Controllers
{
    [Authorize]
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        //private HouseHelper houseHelper = new HouseHelper();

        // GET: Households
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(userId);
            var houseId = currentUser.HouseholdId;

            //ViewBag.HouseName = db.Households.Find(houseId).Name;
            //return View(db.Households.Find(houseId).Name.ToList());
            //return View(db.Households.ToList());
            //var houseHolds = db.Households.Find(houseId).Name;
            var houseHolds = db.Households.Where(h => h.Id == houseId);
            return View(houseHolds.ToList());
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Households households = db.Households.Find(id);
            if (households == null)
            {
                return HttpNotFound();
            }
            return View(households);
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Households households)
        {
            if (ModelState.IsValid)
            {
                db.Households.Add(households);
                db.SaveChanges();

                var userId = User.Identity.GetUserId();
                var userInfo = db.Users.Find(userId);
                //var userManager = new UserManager<ApplicationUser>();
                //userManager.AddToRole(userId, "Member");


                // Create & initialize budget for new house;
                Budgets Budget = new Budgets();
                Budget.Name = households.Name + " Budget";
                Budget.HouseholdId = households.Id;
                Budget.Income = 0;
                Budget.Expense = 0;
                Budget.Created = DateTime.Now;

                //Add new budget to database
                db.Budgets.Add(Budget);

                // Assign user to newly created household ID
                userInfo.HouseholdId = households.Id;

                // Add user to house role "Member"
                userManager.AddToRole(userId, "Member");
                db.SaveChanges();   


                return RedirectToAction("Index", "Transactions");
            }

            return View(households);
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Households households = db.Households.Find(id);
            if (households == null)
            {
                return HttpNotFound();
            }
            return View(households);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Households households)
        {
            if (ModelState.IsValid)
            {
                db.Entry(households).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(households);
        }


        // Get: Households/Leave
        public ActionResult Leave()
        {
            var user = User.Identity.GetUserId();
            var userInfo = db.Users.Find(user);
            var houseHold = db.Households.Find(userInfo.HouseholdId);
            ViewBag.HouseName = db.Households.Find(userInfo.HouseholdId).Name;
            return View(houseHold);
        }

        // Post: Households/Leave
        [HttpPost]
        public ActionResult Leave(bool leaveConfirmation)
        {
            if (leaveConfirmation == false)
            {
                return View();
            }
            var user = User.Identity.GetUserId();
            var currentUser = db.Users.Find(user);
            var houseHold = db.Households.Find(currentUser.HouseholdId);
            houseHold.Users.Remove(currentUser);
            db.SaveChanges();
            return View();
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Households households = db.Households.Find(id);
            if (households == null)
            {
                return HttpNotFound();
            }
            return View(households);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Households households = db.Households.Find(id);
            db.Households.Remove(households);
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
