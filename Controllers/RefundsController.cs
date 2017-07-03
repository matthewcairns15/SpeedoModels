using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpeedoModels.Models;

namespace SpeedoModels.Controllers
{
    public class RefundsController : Controller
    {
        /// <summary>
        /// The database
        /// </summary>
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Refunds
        /// <summary>
        /// returns the refunds to list and Index View.
        /// </summary>
        /// <returns>View(db.Refunds.ToList())</returns>
        public ActionResult Index()
        {
            return View(db.Refunds.ToList());
        }

        // GET: Refunds/Details/5
        /// <summary>
        /// Details for the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(refunds)</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the refund
            Refunds refunds = db.Refunds.Find(id);
            if (refunds == null)
            {
                return HttpNotFound();
            }
            return View(refunds);
        }

        // GET: Refunds/Create
        /// <summary>
        /// retuns the create View.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Create()
        {
            return View();
        }

        // POST: Refunds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Creates the specified refund.
        /// </summary>
        /// <param name="refunds">The refunds.</param>
        /// <returns>RedirectToAction("Index"),View(refunds)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RefundId,transactionId,UserEmail,RefundTotal")] Refunds refunds)
        {
            if (ModelState.IsValid)
            {
                //adds a refund to the database and saves the changes
                db.Refunds.Add(refunds);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(refunds);
        }

        // GET: Refunds/Edit/5
        /// <summary>
        /// Edits the specified refund.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(refunds)</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the refund
            Refunds refunds = db.Refunds.Find(id);
            if (refunds == null)
            {
                return HttpNotFound();
            }
            return View(refunds);
        }

        // POST: Refunds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Edits the specified refund.
        /// </summary>
        /// <param name="refunds">The refunds.</param>
        /// <returns>RedirectToAction("Index"),View(refunds)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RefundId,transactionId,UserEmail,RefundTotal")] Refunds refunds)
        {
            if (ModelState.IsValid)
            {
                //saves the changes
                db.Entry(refunds).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(refunds);
        }

        // GET: Refunds/Delete/5
        /// <summary>
        /// Deletes the specified refund.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(refunds)</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the refund
            Refunds refunds = db.Refunds.Find(id);
            if (refunds == null)
            {
                return HttpNotFound();
            }
            return View(refunds);
        }

        // POST: Refunds/Delete/5
        /// <summary>
        /// Deletes the refund.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>RedirectToAction("Index")</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //finds the refund
            Refunds refunds = db.Refunds.Find(id);
            //removes the refund from the database and saves the changes
            db.Refunds.Remove(refunds);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
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
