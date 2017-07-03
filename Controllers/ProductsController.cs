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
    public class ProductsController : Controller
    {
        /// <summary>
        /// The database
        /// </summary>
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products
        /// <summary>
        /// displays the products in a list and returns the Index View.
        /// </summary>
        /// <returns>View(db.Products.ToList())</returns>
        public ActionResult Index()
        {
            return View(db.Products.ToList());
            
            
        }



        // GET: Products
        /// <summary>
        /// retuns the products in a list and Admin View.
        /// </summary>
        /// <returns>View(db.Products.ToList())</returns>
        public ActionResult Admin()
        {
            return View(db.Products.ToList());
        }

        /// <summary>
        /// finds products with low stock for a report.
        /// </summary>
        /// <returns>View(products.ToList())</returns>
        public ActionResult LowStockReport()
        {



            //checks for products with a stock less than 5
            var products = from o in db.Products
                         where o.productStock < 5
                         select o;

            return View(products.ToList());
        }


        /// <summary>
        /// queries the database for products with low stock and returns them to a view without the layout
        /// </summary>
        /// <returns>View(products.ToList())</returns>
        public ActionResult LowStockReportPDF()
        {


            //checks for products with a stock less than 5


            var products = from o in db.Products
                           where o.productStock < 5
                           select o;

            return View(products.ToList());
        }
        /// <summary>
        /// Checks the products db for any products with a stock thats less than 5
        /// and then exports them to pdf
        /// </summary>
        /// <returns>new Rotativa.ViewAsPdf("LowStockReportpdf", products) { FileName = "LowStock.PDF" }</returns>


        public ActionResult ExportAdminReportToPDF()
        {

            var products = from o in db.Products
                           where o.productStock < 5
                           select o;

            return new Rotativa.ViewAsPdf("LowStockReportpdf", products) { FileName = "LowStock.PDF" };
        }



        // GET: Products/Details/5
        /// <summary>
        /// Details for the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(product)</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the product
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        /// <summary>
        ///  retuns the create View.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Creates the specified product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns>RedirectToAction("Index"),View(product)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "productId,productName,productDescription,Price,productColour,productStock")] Product product)
        {
            if (ModelState.IsValid)
            {
                //adds the product to the database and saves the changes
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(product)</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the product
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }




        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Edits the specified product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns>RedirectToAction("Index"),View(product)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "productId,productName,productDescription,Price,productColour,productStock")] Product product)
        {
            if (ModelState.IsValid)
            {
                //saves the changes
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }



        // GET: Products/Delete/5
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(product)</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the product to be deleted
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        /// <summary>
        /// Deletes the confirmed product.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>RedirectToAction("Index")</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {            
            //finds the product to be deleted
            Product product = db.Products.Find(id);
            //removes the product and saves the changes
            db.Products.Remove(product);
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
