using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpeedoModels.Models;
using System.Threading.Tasks;

namespace SpeedoModels.Controllers
{
    public class CompleteOrdersController : Controller
    {
        /// <summary>
        /// The database
        /// </summary>
        private ApplicationDbContext db = new ApplicationDbContext();
        /// <summary>
        /// Returns the complete orders in a list
        /// </summary>
        /// <returns>
        /// View(db.CompleteOrders.ToList())
        /// </returns>
        // GET: CompleteOrders
        public ActionResult Index()
        {
            return View(db.CompleteOrders.ToList());
        }
        ApplicationDbContext storeDB = new ApplicationDbContext();

        /// <summary>
        /// Cannotrefunds this instance.
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        public ActionResult cannotrefund()
        {
            return View();
        }
        /// <summary>
        /// Returns View for Refund notification
        /// </summary>
        /// <returns>
        /// View
        /// </returns>

        public ActionResult Refundnotification()
        {
            return View();
        }
        /// <summary>
        /// Returns View for Somethingwentwrong.
        /// </summary>
        /// <returns>
        /// View
        /// </returns>

        public ActionResult Somethingwentwrong()
        {
            return View();
        }
        /// <summary>
        /// Returns View for AlreadyDelivered
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        public ActionResult AlreadyDelivered()
        {
            return View();
        }

        /// <summary>
        /// Displays all compete orders made within the last 7 days in a list
        /// </summary>
        /// <returns>
        /// View(orders.ToList())
        /// </returns>

        public ActionResult AdminReport()
        {
            //queries the completeorders database for all orders made within the last 7 days 
            DateTime v = DateTime.Now.AddDays(-7);

            var orders = from o in db.CompleteOrders
                         where o.COrderDate > v
                         select o;



            //make it so it only displays the last weeks 


            return View(orders.ToList());

        }

        /// <summary>
        /// Displays all compete orders made within the last 7 days in a list on a page without the company layout
        /// </summary>
        /// <returns>
        /// View(orders.ToList())
        /// </returns>
        public ActionResult AdminReportpdf()
        {


            DateTime v = DateTime.Now.AddDays(-7);

            var orders = from o in db.CompleteOrders
                         where o.COrderDate > v
                         select o;
            //make it so it only displays the orders made in the last week 


            return View(orders.ToList());

        }


        /// <summary>
        /// Exports the Report to PDF
        /// </summary>
        /// <returns>
        /// new Rotativa.ViewAsPdf("AdminReportpdf", orders){ FileName = "Admin.PDF" }
        /// </returns>

        public ActionResult ExportAdminReportToPDF()
        {
            //make it so it only displays the last weeks 
            DateTime v = DateTime.Now.AddDays(-7);

            var orders = from o in db.CompleteOrders
                         where o.COrderDate > v
                         select o;
            
            
            return new Rotativa.ViewAsPdf("AdminReportpdf", orders){ FileName = "Admin.PDF" };
        }






        // GET: Orders/DeleteMO/5
        /// <summary>
        /// Gets the complete order details to be deleted
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// new HttpStatusCodeResult(HttpStatusCode.BadRequest),View(order)
        /// </returns>


        public async Task<ActionResult> DeleteMO(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompleteOrders order = await db.CompleteOrders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        /// <summary>
        /// Deletes the selected order and gives an appropriate refund, unless the order has already been delivered
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// View("cannotrefund"),View("Somethingwentwrong"), View("Refundnotification")
        /// </returns>

        // POST: Orders/Delete/5
        [HttpPost, ActionName("DeleteMO")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteMOConfirmed(int id)
        {
            CompleteOrders order = await db.CompleteOrders.FindAsync(id);

            var refund = new Refunds();
            TryUpdateModel(refund);


            try
            {
                //checks that the order hasnt alrady been delivered
                if(order.CDeliveryDate < DateTime.Now)
                {
                    return View("AlreadyDelivered");
                }
                // checks if the order has been made within the last 48 hours but has 48 hours or less until its delivered
                else if (order.COrderDate > DateTime.Now.AddHours(-48) && order.CDeliveryDate < DateTime.Now.AddHours(48))
                {
                    refund.transactionId = order.transactionId;
                    refund.UserEmail = order.CEmail;
                    decimal refundtot = order.CTotal;

                    //Calculates 75% of the refund total
                    refundtot = ((decimal)75 / 100) * refundtot;
                    refund.RefundTotal = refundtot;

                    //adds the refund to the database
                    storeDB.Refunds.Add(refund);
                    await storeDB.SaveChangesAsync();

                }
                //checks that the order was made within the last 48 hours
                else if (order.COrderDate > DateTime.Now.AddHours(-48))
                {
                    //gets the refund details
                    refund.transactionId = order.transactionId;
                    refund.UserEmail = order.CEmail;
                    refund.RefundTotal = order.CTotal;                    

                    //adds the refund to the database
                    storeDB.Refunds.Add(refund);
                    await storeDB.SaveChangesAsync();

                }

                else
                {
                     return View("cannotrefund");
                }
            }
            catch
            {
                return View("Somethingwentwrong");
            }

            //removes the order from the database
                db.CompleteOrders.Remove(order);
            await db.SaveChangesAsync();
            return View("Refundnotification");
        }

        /// <summary>
        /// Displays the Orders made by the current user
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="currentFilter">The current filter.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="page">The page.</param>
        /// <param name="order">The order.</param>
        /// <returns>
        /// View(orders.ToList())
        /// </returns>

        // GET: Orders and filters so that it only displays the current users orders
        public ActionResult MyOrders(string sortOrder, string currentFilter, string searchString, int? page, CompleteOrders order)
        {



            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

            //Displays the orders in which the order email matches the application users email

            var orders = from o in db.CompleteOrders
                         where o.CEmail == User.Identity.Name
                         select o;


            switch (sortOrder)
            {
                case "name_desc":
                    orders = orders.OrderByDescending(s => s.CFirstName);
                    break;
                case "Price":
                    orders = orders.OrderBy(s => s.CTotal);
                    break;
                case "price_desc":
                    orders = orders.OrderByDescending(s => s.CTotal);
                    break;
                default:  // Name ascending 
                    orders = orders.OrderBy(s => s.CFirstName);

                    break;
            }


            return View(orders.ToList());
        
        }


        // GET: CompleteOrders/Details/5
        /// <summary>
        /// Gets the details of a complete order
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// new HttpStatusCodeResult(HttpStatusCode.BadRequest), View(completeOrders)
        /// </returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompleteOrders completeOrders = db.CompleteOrders.Find(id);
            if (completeOrders == null)
            {
                return HttpNotFound();
            }
            return View(completeOrders);
        }

        /// <summary>
        /// Returns create View.
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        // GET: CompleteOrders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CompleteOrders/Create
        /// <summary>
        /// Creates a complete order
        /// </summary>
        /// <param name="completeOrders">The complete orders.</param>
        /// <returns>
        /// RedirectToAction("Index"), View(completeOrders)
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CompleteOrders completeOrders)
        {
            if (ModelState.IsValid)
            {
                //adds the complete order to the database
                db.CompleteOrders.Add(completeOrders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(completeOrders);
        }

        // GET: CompleteOrders/Edit/5
        /// <summary>
        /// Edits a complete order
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// new HttpStatusCodeResult(HttpStatusCode.BadRequest,return HttpNotFound(),View(completeOrders)
        /// </returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the order to be edited
            CompleteOrders completeOrders = db.CompleteOrders.Find(id);
            if (completeOrders == null)
            {
                return HttpNotFound();
            }
            return View(completeOrders);
        }

        // POST: CompleteOrders/Edit/5

        /// <summary>
        /// Edits the specified complete orders.
        /// </summary>
        /// <param name="completeOrders">The complete orders.</param>
        /// <returns>
        /// RedirectToAction("Index"), View(completeOrders)
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CompleteOrders completeOrders)
        {
            if (ModelState.IsValid)
            {
                //saves the edits
                db.Entry(completeOrders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(completeOrders);
        }

        // GET: CompleteOrders/Delete/5
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>HttpNotFound(),View(completeOrders)</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the complete order to be deleted
            CompleteOrders completeOrders = db.CompleteOrders.Find(id);
            if (completeOrders == null)
            {
                return HttpNotFound();
            }
            return View(completeOrders);
        }

        // POST: CompleteOrders/Delete/5

        /// <summary>
        /// Deletes the complete order.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>RedirectToAction("Index")</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompleteOrders completeOrders = db.CompleteOrders.Find(id);
            // removes the complete order and saves the changes
            db.CompleteOrders.Remove(completeOrders);
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
