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
using System.Threading.Tasks;

namespace FinancialPortal.Controllers
{
    [Authorize]
    public class InvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //private HouseHelper houseHelper = new HouseHelper();

        // GET: Invitations
        public ActionResult Index()
        {
            var invitations = db.Invitations.Include(i => i.Household);
            return View(invitations.ToList());
        }

        // GET: Invitations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitations invitations = db.Invitations.Find(id);
            if (invitations == null)
            {
                return HttpNotFound();
            }
            return View(invitations);
        }

        // GET: Invitations/Create
        public ActionResult Create()
        {

            Invitations invitation = new Invitations();
            var userId = User.Identity.GetUserId();
            var userInfo = db.Users.Find(userId);

            // Creating unique invite code & converting to string using "N" format
            //invitation.InvCd = Guid.NewGuid().ToString();
            invitation.HouseholdId = userInfo.HouseholdId;
            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View(invitation);
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "Id,HouseholdId,InviteEmail,InvCd,InviteCreated,InviteModified,InviteExpiration")] Invitations invitations)
                    public async Task<ActionResult> Create([Bind(Include = "Id,HouseholdId,InviteEmail,InvCd,InviteCreated,InviteModified,InviteExpiration")] Invitations invitations)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var userInfo = db.Users.Find(userId);
 

                invitations.InviteCreated = DateTime.Now;
                invitations.InviteModified = DateTime.Now;
                invitations.InviteExpiration = DateTime.Now.AddHours(48);
                invitations.InvCd = Guid.NewGuid().ToString();
                invitations.HouseholdId = userInfo.HouseholdId;

                db.Invitations.Add(invitations);
                db.SaveChanges();

                // Locate household invitation name - to be emailed below
                var houseName = db.Households.FirstOrDefault(h => h.Id == invitations.HouseholdId).Name;

                await Send(invitations);



                //Send Notification
                //var callbackUrlRegister = Url.Action("Register", "Account", new { InvCd = invitations.InvCd}, protocol: Request.Url.Scheme);
                //var callbackUrlJoin = Url.Action("Join", "Invitations", new { InvCd = invitations.InvCd }, protocol: Request.Url.Scheme);
                //var emailSvc = new EmailService();
                //var svc = new EmailService();
                //var msg = new IdentityMessage();
                //msg.Destination = invitations.InviteEmail;
                //msg.Subject = "Ivitation:  Financial Portal";
                ////msg.Body = ("You have been invited to join the following Financial Portal household: " + houseName + ". " + "Please use the following invitation code when registering: " + invitations.InvCd + ".");
                //msg.Body = ("You have been invited to join a household on Financial Portal: " + houseName + ". "
                //    + "Please click <a href=\"" + callbackUrlRegister + "\">here</a> " + "to register.  If you have already registered, you may <a href=\"" + callbackUrlJoin + "\">join</a> "
                //    + "the household. The following invitation code will expire in 24 hours: " + invitations.InvCd);
                ////await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                //await svc.SendAsync(msg);
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", invitations.HouseholdId);
            return View(invitations);
        }


        // GET: Invitations/Join
        public ActionResult Join(/*string InvCd*/)
        {
            //var invitation = new Invitations();
            //invitation.InvCd = InvCd;
            return View();

        }
            

            // POST: Invitations/Join
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Join([Bind(Include = "Id,HouseholdId,InviteEmail,InvCd,InviteCreated,InviteModified,InviteExpiration")] Invitations invitations)
        {

            var userId = User.Identity.GetUserId();
            var userInfo = db.Users.Find(userId);
            
            // Retrieve original invitation Id by looking up invitation code entered by user
            var inviteId = db.Invitations.FirstOrDefault(i => i.InvCd == invitations.InvCd).Id;

            // Pull all invitation info by invitation Id
            var inviteInfo = db.Invitations.Find(inviteId);

            // Sanity check - ensure email entered by user matches invitation email
            if (invitations.InviteEmail != inviteInfo.InviteEmail)
            {
                ViewBag.Message = "Error:  Email entered does not match invitaiton email on file";
                return View();    // ==> Need to customize error message & redirect <== //
            }

            // Find authenticated user by UserId
            var currentUser = db.Users.Find(User.Identity.GetUserId());


            // Ensure invitation is still valid
            if (DateTime.Now > inviteInfo.InviteExpiration && inviteInfo.IsRegistered == false)
            {
                ViewBag.Message = "Error:  Invitation expired!";
                return View(); 
            }

            if (DateTime.Now > inviteInfo.InviteExpiration && inviteInfo.IsRegistered == true)
            {
                ViewBag.Message = "Error:  Already registered - invitation expired";
                return View();  
            }

            // Set invite expiration to currente date/time and registered flag to true
            inviteInfo.IsRegistered = true;
            inviteInfo.InviteExpiration = DateTime.Now;
            inviteInfo.InviteModified = DateTime.Now;
            userInfo.HouseholdId = inviteInfo.HouseholdId;
            db.SaveChanges();

            return RedirectToAction("Index", "Transactions");

        }


        // GET: Invitations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitations invitations = db.Invitations.Find(id);
            if (invitations == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", invitations.HouseholdId);
            return View(invitations);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,HouseholdId,InviteEmail,InvCd,InviteCreated,InviteModified,InviteExpiration")] Invitations invitations)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", invitations.HouseholdId);
                return View(invitations);
            }

            db.Entry(invitations).State = EntityState.Modified;

            // Reset invitation expiration given Invitation update
            invitations.InviteExpiration = DateTime.Now.AddHours(24);
            invitations.InviteModified = DateTime.Now;
            db.SaveChanges();

            //Send notification
            await Send(invitations);

            return RedirectToAction("Index");
        }

        // GET: Invitations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitations invitations = db.Invitations.Find(id);
            if (invitations == null)
            {
                return HttpNotFound();
            }
            return View(invitations);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invitations invitations = db.Invitations.Find(id);
            db.Invitations.Remove(invitations);
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

        // Send invitation email notification
        public async Task Send(Invitations invitations)
        {
            var houseName = db.Households.FirstOrDefault(h => h.Id == invitations.HouseholdId).Name;
            var callbackUrlRegister = Url.Action("Register", "Account", new { InvCd = invitations.InvCd }, protocol: Request.Url.Scheme);
            var callbackUrlJoin = Url.Action("Join", "Invitations", new { InvCd = invitations.InvCd }, protocol: Request.Url.Scheme);
            var emailSvc = new EmailService();
            var svc = new EmailService();
            var msg = new IdentityMessage();
            msg.Destination = invitations.InviteEmail;
            msg.Subject = "Financial Portal Invitation";
            //msg.Body = ("You have been invited to join the following Financial Portal household: " + houseName + ". " + "Please use the following invitation code when registering: " + invitations.InvCd + ".");
            msg.Body = ("You have been invited to join a household in Sweep The Change Household Budgeter: " + houseName + ". "
                + "Please click <a href=\"" + callbackUrlRegister + "\">here</a> " + "to register. "
                + "The following invitation code expires in 48 hours and will be needed to join: " + invitations.InvCd);
            //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            await svc.SendAsync(msg);
            //return RedirectToAction("Index");
            return;
        }
    }
}
 