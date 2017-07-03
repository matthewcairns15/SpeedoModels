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
    public class OrderBackupsController : Controller
    {
        /// <summary>
        /// The database
        /// </summary>
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OrderBackups
        /// <summary>
        /// Returns index view with orderbackups db in list format 
        /// </summary>
        /// <returns>View(db.OrderBackups.ToList())</returns>
        public ActionResult Index()
        {
            return View(db.OrderBackups.ToList());
        }

        // GET: OrderBackups/Details/5
        /// <summary>
        /// Details the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(orderBackup) </returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the orderbackup
            OrderBackup orderBackup = db.OrderBackups.Find(id);
            if (orderBackup == null)
            {
                return HttpNotFound();
            }
            return View(orderBackup);
        }

        // GET: OrderBackups/Create
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderBackups/Create
        /// <summary>
        /// Backs up the completeOrders database.
        /// </summary>
        /// <param name="orderBackup">The order backup.</param>
        /// <returns>RedirectToAction("Index")</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderBackup orderBackup)
        {
            //Removes the old rows in the database so the new database can be backed up
            foreach (var row in db.OrderBackups)
            {
                db.OrderBackups.Remove(row);
            }
            //Adds the Orders from the Orders database into the Orders backup database



            foreach (var row in db.CompleteOrders)
            {
                orderBackup = new OrderBackup();
                orderBackup.transactionId = row.transactionId;
                orderBackup.Address = row.CAddress;
                orderBackup.City = row.CCity;
                orderBackup.Country = row.CCountry;
                orderBackup.DeliveryDate = row.CDeliveryDate;
                orderBackup.DeliveryMethod = row.CDeliveryMethod;
                orderBackup.Email = row.CEmail;
                orderBackup.FirstName = row.CFirstName;
                orderBackup.HasPaid = row.CHasPaid;
                orderBackup.LastName = row.CLastName;
                orderBackup.OrderDate = row.COrderDate;
                orderBackup.OrderId = row.COrderId;
                orderBackup.Phone = row.CPhone;
                orderBackup.PostalCode = row.CPostalCode;
                orderBackup.SaveInfo = row.CSaveInfo;
                orderBackup.State = row.CState;
                orderBackup.Total = row.CTotal;
                orderBackup.Username = row.CUsername;

                db.OrderBackups.Add(orderBackup);
            }
                            db.SaveChanges();

                return RedirectToAction("Index");
           


        }

        // GET: OrderBackups/Edit/5
        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(orderBackup)</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderBackup orderBackup = db.OrderBackups.Find(id);
            if (orderBackup == null)
            {
                return HttpNotFound();
            }
            return View(orderBackup);
        }

        // POST: OrderBackups/Edit/5
        /// <summary>
        /// Edits the specified order backup.
        /// </summary>
        /// <param name="orderBackup">The order backup.</param>
        /// <returns>RedirectToAction("Index"),View(orderBackup)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderBackup orderBackup)
        {
            if (ModelState.IsValid)
            {
                //saves the edits
                db.Entry(orderBackup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(orderBackup);
        }

        // GET: OrderBackups/Delete/5
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(orderBackup)</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the order backup to be deleted
            OrderBackup orderBackup = db.OrderBackups.Find(id);
            if (orderBackup == null)
            {
                return HttpNotFound();
            }
            return View(orderBackup);
        }

        // POST: OrderBackups/Delete/5
        /// <summary>
        /// Deletes the confirmed backup order.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>RedirectToAction("Index")</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {          
            //finds the order backup to be deleted
            OrderBackup orderBackup = db.OrderBackups.Find(id);
            //removes the orderbackup from the database and saves the changes
            db.OrderBackups.Remove(orderBackup);
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
