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
    public class ProductBackupsController : Controller
    {
        /// <summary>
        /// The database
        /// </summary>
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductBackups
        /// <summary>
        /// retuns the productsbackup db in a list and index View.
        /// </summary>
        /// <returns>View(db.ProductBackups.ToList())</returns>
        public ActionResult Index()
        {
            return View(db.ProductBackups.ToList());
        }

        // GET: ProductBackups/Details/5
        /// <summary>
        /// Details the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(productBackup)</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the productBackup
            ProductBackup productBackup = db.ProductBackups.Find(id);
            if (productBackup == null)
            {
                return HttpNotFound();
            }
            return View(productBackup);
        }

        // GET: ProductBackups/Create
        /// <summary>
        /// retuns the create View.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductBackups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Creates the specified product backup.
        /// </summary>
        /// <param name="productBackup">The product backup.</param>
        /// <returns>RedirectToAction("Index")</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "productId,productName,productDescription,Price,productColour,productStock")] ProductBackup productBackup)
        {
            //Removes the old rows in the database so the new database can be backed up


            foreach (var row in db.ProductBackups)
            {
                db.ProductBackups.Remove(row);
            }

            //Adds the products from the products database into the product backup database
            foreach (var row in db.Products)
            {
                productBackup = new ProductBackup();
                productBackup.Price = row.Price;
                productBackup.productColour = row.productColour;
                productBackup.productDescription = row.productDescription;
                productBackup.productId = row.productId;
                productBackup.productName = row.productName;
                productBackup.productStock = row.productStock;

                db.ProductBackups.Add(productBackup);
            }
                db.SaveChanges();
                return RedirectToAction("Index");
            

            
        }

        // GET: ProductBackups/Edit/5
        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(productBackup)</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductBackup productBackup = db.ProductBackups.Find(id);
            if (productBackup == null)
            {
                return HttpNotFound();
            }
            return View(productBackup);
        }

        // POST: ProductBackups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Edits the specified product backup.
        /// </summary>
        /// <param name="productBackup">The product backup.</param>
        /// <returns>RedirectToAction("Index"),View(productBackup)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "productId,productName,productDescription,Price,productColour,productStock")] ProductBackup productBackup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productBackup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productBackup);
        }

        // GET: ProductBackups/Delete/5
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(productBackup)</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductBackup productBackup = db.ProductBackups.Find(id);
            if (productBackup == null)
            {
                return HttpNotFound();
            }
            return View(productBackup);
        }

        // POST: ProductBackups/Delete/5
        /// <summary>
        /// Deletes the confirmed backup product.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>RedirectToAction("Index")</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //finds the productBackup
            ProductBackup productBackup = db.ProductBackups.Find(id);
            db.ProductBackups.Remove(productBackup);
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
