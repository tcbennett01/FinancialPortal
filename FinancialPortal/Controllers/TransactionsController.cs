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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var user = User.Identity.GetUserId();
            var currentUser = db.Users.Find(user);
            var house = db.Households.Where(h => h.Id == currentUser.HouseholdId);

            var tod = System.DateTimeOffset.Now;
            var userAccts = db.Accounts.Where(p => p.HouseholdId == currentUser.HouseholdId);
            
            // Commented out logic that pulls transactions for current month - controlling demo
            //var transactions = userAccts.SelectMany(t => t.Transactions).Where(d => d.CreateDate.Year == tod.Year
            //                                                                   && d.CreateDate.Month == tod.Month);

            // Hard coding recent transactions to be >= Nov 2016 - demo purposes
            var transactions = userAccts.SelectMany(t => t.Transactions).Where(d => (d.CreateDate.Year == 2016
                                                                   && d.CreateDate.Month >= 11) 
                                                                   || d.CreateDate.Year >= 2017);
            return View(transactions.ToList());
        }

        // GET: Transactions
        public ActionResult AccountTrans(int? id)
        {
            var user = User.Identity.GetUserId();
            var currentUser = db.Users.Find(user);
            var house = db.Households.Where(h => h.Id == currentUser.HouseholdId);
            var acctBal = db.Accounts.Where(a => a.Id == id).Select(b => b.Balance);
            

            var userAccts = db.Accounts.Where(p => p.HouseholdId == currentUser.HouseholdId);
            var transactions = userAccts.SelectMany(t => t.Transactions.Where(i => i.AccountId == id));

            ViewBag.Bal = acctBal.Sum();
            ViewBag.AcctName = db.Accounts.Find(id).Name;
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transactions transactions = db.Transactions.Find(id);
            if (transactions == null)
            {
                return HttpNotFound();
            }
            return View(transactions);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            Accounts accounts = new Accounts();

            var userId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(userId);
            var houseId = currentUser.HouseholdId;
            var userAccts = db.Accounts.Where(a => a.HouseholdId == houseId);

            //ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name");
            ViewBag.AccountId = new SelectList(userAccts, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", "Group", 1);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,CreateDate,ModifyDate,Amount,TransactionTypeId,CategoryId,Description,EnteredById,IsReconciled,ExcludeBudgetItem")] Transactions transactions)
        {
            var userId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(userId);
            var currentAcct = db.Accounts.Find(transactions.AccountId);

            // Set modify date & user Id
            transactions.ModifyDate = DateTime.Now;
            transactions.EnteredById = userId;

            // Set catetory type - < 7 = income, else expense
            transactions.TransactionTypeId = (transactions.CategoryId < 7) ? 1 : 2;

            // Change transaction amount sign if necessary
            if ((transactions.TransactionTypeId == 1 && transactions.Amount < 0)
                || (transactions.TransactionTypeId == 2 && transactions.Amount > 0))
            {
                transactions.Amount *= -1;
            }

            // Check reconciled flag & set reconciled amount
            //transactions.ReconciledAmount = (transactions.IsReconciled == true) ? transactions.Amount : 0;

            // Update Balances
            currentAcct.Balance += transactions.Amount;
            currentAcct.ReconciledBalance += (transactions.IsReconciled == true) ? transactions.Amount : 0;

            if (ModelState.IsValid)
            {
                db.Transactions.Add(transactions);
                //db.Accounts.Add(currentAcct);
                db.SaveChanges();
                //return RedirectToAction("Index");
                return RedirectToAction("AccountTrans", "Transactions", new { id = transactions.AccountId });
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transactions.AccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", "Group", transactions.CategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name", transactions.TransactionTypeId);
            return View(transactions);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transactions transactions = db.Transactions.Find(id);
            if (transactions == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transactions.AccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transactions.CategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name", transactions.TransactionTypeId);
            return View(transactions);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,CreateDate,ModifyDate,Amount,TransactionTypeId,CategoryId,Description,EnteredById,IsReconciled,IsVoid,VoidAmount")] Transactions updateTrans)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", updateTrans.AccountId);
                ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", updateTrans.CategoryId);
                ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name", updateTrans.TransactionTypeId);
                return View(updateTrans);
            }

            var userId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(userId);

            // Retrive old transaction information
            var oldTrans = db.Transactions.Find(updateTrans.Id);

            // Check transaction sign - credits positive & debits negative
            if ((updateTrans.CategoryId < 7 && updateTrans.Amount < 0)
                || (updateTrans.CategoryId >= 7 && updateTrans.Amount > 0))
            {
                updateTrans.Amount *= -1;
            }

            // If Transaction amount change or transaction Id type change
            if ((oldTrans.Amount != updateTrans.Amount)
                || (oldTrans.TransactionTypeId != updateTrans.TransactionTypeId))
            {
                UpdateAcctBalance(oldTrans, updateTrans);
            }

            // Check reconcile flag - no to yes
            if (oldTrans.IsReconciled == false && updateTrans.IsReconciled == true)
            {
                AddReconciled(updateTrans);
            }

            // Check reconcile flag - yes to no
            if (oldTrans.IsReconciled == true && updateTrans.IsReconciled == false)
            {
                RemoveReconciled(oldTrans, updateTrans);
            }

            UpdateTransaction(oldTrans, updateTrans);

            //return RedirectToAction("Index");
            return RedirectToAction("AccountTrans", "Transactions", new { id = updateTrans.AccountId });

        }

        public void UpdateAcctBalance(Transactions oldTrans, Transactions updateTrans)
        {
            var currentAcct = db.Accounts.Find(updateTrans.AccountId);

            // Back out old transaction amount & add new amount
            currentAcct.Balance -= oldTrans.Amount;
            currentAcct.Balance += updateTrans.Amount;

            // Check to see if reconcile flag was 'yes' and currently 'yes;
            if (oldTrans.IsReconciled == true && updateTrans.IsReconciled == true)
            {
                currentAcct.ReconciledBalance -= oldTrans.Amount;
                currentAcct.ReconciledBalance += updateTrans.Amount;
            }

            if (ModelState.IsValid)
            {
                db.Entry(currentAcct).State = EntityState.Modified;
                db.SaveChanges();
                return;
            }
        }

        public void AddReconciled(Transactions updateTrans)
        {
            var currentAcct = db.Accounts.Find(updateTrans.AccountId);
            currentAcct.ReconciledBalance += updateTrans.Amount;

            if (ModelState.IsValid)
            {
                db.Entry(currentAcct).State = EntityState.Modified;
                db.SaveChanges();
            }
            return;
        }

        public void RemoveReconciled(Transactions oldTrans, Transactions updateTrans)
        {
            var currentAcct = db.Accounts.Find(updateTrans.AccountId);
            currentAcct.ReconciledBalance -= oldTrans.Amount;
            if (ModelState.IsValid)
            {
                db.Entry(currentAcct).State = EntityState.Modified;
                db.SaveChanges();
            }
            return;
        }

        public void UpdateTransaction(Transactions oldTrans, Transactions updateTrans)
        {
            if (oldTrans.IsReconciled != updateTrans.IsReconciled)
            {
                oldTrans.IsReconciled = updateTrans.IsReconciled;
                //oldTrans.ReconciledAmount = updateTrans.ReconciledAmount;
            }

            if (oldTrans.CategoryId != updateTrans.CategoryId)
            {
                oldTrans.CategoryId = updateTrans.CategoryId;
                oldTrans.TransactionTypeId = (oldTrans.TransactionTypeId == 1) ? 2 : 1;
            }

            oldTrans.CreateDate = (oldTrans.CreateDate == updateTrans.CreateDate) ? oldTrans.CreateDate : updateTrans.CreateDate;
            oldTrans.Amount = (oldTrans.Amount == updateTrans.Amount) ? oldTrans.Amount : updateTrans.Amount;
            oldTrans.CategoryId = (oldTrans.CategoryId == updateTrans.CategoryId) ? oldTrans.CategoryId : updateTrans.CategoryId;
            oldTrans.Description = (oldTrans.Description == updateTrans.Description) ? oldTrans.Description : updateTrans.Description;
            oldTrans.ModifyDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(oldTrans).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transactions transactions = db.Transactions.Find(id);
            if (transactions == null)
            {
                return HttpNotFound();
            }
            return View(transactions);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var transactions = db.Transactions.Find(id);
            var accounts = db.Accounts.Find(transactions.AccountId);

            // If trans not previously voided, adjust balance & reconciled balance accordingly
            if (!transactions.IsVoid)
            {
                accounts.Balance -= transactions.Amount;
                accounts.ReconciledBalance = (transactions.IsReconciled) ? accounts.ReconciledBalance - transactions.Amount : accounts.ReconciledBalance;
            }

            transactions.Deleted = true;
            db.Entry(transactions).State = EntityState.Modified;
            db.SaveChanges();
            //return RedirectToAction("Index");
            return RedirectToAction("AccountTrans", "Transactions", new { id = transactions.AccountId });
        }

        // GET: Transactions/Void/5
        public ActionResult Void(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transactions transactions = db.Transactions.Find(id);
            if (transactions == null)
            {
                return HttpNotFound();
            }
            return View(transactions);
        }

        //POST: Transactions/Void/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Void([Bind(Include = "Id,AccountId,CreateDate,ModifyDate,Amount,TransactionTypeId,CategoryId,Description,EnteredById,IsReconciled, IsVoid")] Transactions transDetails)
        {
            var oldTrans = db.Transactions.Find(transDetails.Id);

            if (!oldTrans.IsVoid && transDetails.IsVoid)
            {
                AddVoid(transDetails);
            }
            else if (oldTrans.IsVoid && !transDetails.IsVoid)
            {
                RemoveVoid(transDetails);
            }

            //return RedirectToAction("Index");
            return RedirectToAction("AccountTrans", "Transactions", new { id = transDetails.AccountId });
        }

        public void AddVoid(Transactions transDetail)
        {
            var oldTrans = db.Transactions.Find(transDetail.Id);

            // Update account balances
            var acctBal = db.Accounts.Find(oldTrans.AccountId);
            acctBal.Balance -= oldTrans.Amount;
            acctBal.ReconciledBalance = (oldTrans.IsReconciled == true) ? acctBal.ReconciledBalance -= oldTrans.Amount : acctBal.ReconciledBalance;

            // Update transaction amounts
            oldTrans.IsVoid = transDetail.IsVoid;
            oldTrans.TransactionTypeId = 3;
            oldTrans.VoidAmount = oldTrans.Amount;
            oldTrans.Amount = 0;

            db.Entry(acctBal).State = EntityState.Modified;
            db.Entry(oldTrans).State = EntityState.Modified;
            db.SaveChanges();
            return;
        }

        public void RemoveVoid(Transactions transDetail)
        {
            var oldTrans = db.Transactions.Find(transDetail.Id);

            // Update account balances
            var acctBal = db.Accounts.Find(oldTrans.AccountId);
            acctBal.Balance += oldTrans.VoidAmount;
            acctBal.ReconciledBalance = (oldTrans.IsReconciled == true) ? acctBal.ReconciledBalance += oldTrans.VoidAmount : acctBal.ReconciledBalance;

            // Update transaction amounts
            oldTrans.IsVoid = transDetail.IsVoid;
            oldTrans.Amount = oldTrans.VoidAmount; 
            oldTrans.VoidAmount = 0;

            // Set transaction to original type
            oldTrans.TransactionTypeId = (oldTrans.CategoryId < 7) ? 1 : 2;

            db.Entry(acctBal).State = EntityState.Modified;
            db.Entry(oldTrans).State = EntityState.Modified;
            db.SaveChanges();
            return;
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
