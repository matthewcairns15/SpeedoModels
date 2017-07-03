using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using SpeedoModels.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SpeedoModels.Configuration;
using System.Timers;
using System.Net.Mail;

namespace SpeedoModels.Controllers
{
    public class OrdersController : Controller
    {
        /// <summary>
        /// The database
        /// </summary>
        private ApplicationDbContext db = new ApplicationDbContext();





        // GET: Orders
        /// <summary>
        /// Index for the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="currentFilter">The current filter.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="page">The page.</param>
        /// <returns>
        /// View(orders.ToList())
        /// </returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";



            //gets the orders from the orders database
            var orders = from o in db.Orders
            select o;
         
            switch (sortOrder)
            {
                case "name_desc":
                    orders = orders.OrderByDescending(s => s.FirstName);
                    break;
                case "Price":
                    orders = orders.OrderBy(s => s.Total);
                    break;
                case "price_desc":
                    orders = orders.OrderByDescending(s => s.Total);
                    break;
                default:  // Name ascending 
                    orders = orders.OrderBy(s => s.FirstName);
                    break;
            }


            


            return View(orders.ToList());

        }

        /// <summary>
        ///  retuns the View for noorder.
        /// </summary>
        /// <returns> View</returns>
        public ActionResult noorder()
        {
            return View();
        }

        // GET: Orders and filters so that it only displays the current users orders
        /// <summary>
        /// Mies the orders.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="currentFilter">The current filter.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="page">The page.</param>
        /// <param name="order">The order.</param>
        /// <returns>
        /// View(orders.ToList())
        /// </returns>
        public ActionResult MyOrders(string sortOrder, string currentFilter, string searchString, int? page, Order order)
        {
            //order was null so nothing would be displayed
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

            //queries the database to get orders that contain the same email as the current user
            var orders = from o in db.Orders
                         where o.Email == User.Identity.Name
                         select o;


            switch (sortOrder)
            {
                case "name_desc":
                    orders = orders.OrderByDescending(s => s.FirstName);
                    break;
                case "Price":
                    orders = orders.OrderBy(s => s.Total);
                    break;
                case "price_desc":
                    orders = orders.OrderByDescending(s => s.Total);
                    break;
                default:  // Name ascending 
                    orders = orders.OrderBy(s => s.FirstName);

                    break;
            }


            return View(orders.ToList());
           
         
        }


        // GET: Orders/Details/5
        /// <summary>
        /// Details the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(order)</returns>
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the order
            Order order = await db.Orders.FindAsync(id);
            var orderDetails = db.OrderDetails.Where(x => x.OrderId == id);

            order.OrderDetails = await orderDetails.ToListAsync();
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }


        /// <summary>
        /// Returns View for PaymntChoice.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult PaymentChoice()
        {
            return View();
        }



        /// <summary>
        /// retuns the View for Complete.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Complete()
        {
            return View();
        }





        ApplicationDbContext storeDB = new ApplicationDbContext();
        AppConfigurations appConfig = new AppConfigurations();



        /// <summary>
        /// returns tocomplete View.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult toComplete()
        {
            return View();
        }

        /// <summary>
        /// To complete.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>RedirectToAction("Complete", new { id = order.OrderId})</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> toComplete(Order order)
        {
            order = await db.Orders.FindAsync(order.OrderId);


            return RedirectToAction("Complete",
                new { id = order.OrderId });

        }


        // GET: Orders/Edit/5
        /// <summary>
        /// Edits the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(order)</returns>
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the order
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        /// <summary>
        /// Edits the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>RedirectToAction("Index"),View(order)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                //saves the edits
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(order)</returns>
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds the order to be deleted
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/DeleteMO/5


        /// <summary>
        /// Deletes the order.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ew HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(order)</returns>
        public async Task<ActionResult> DeleteMO(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds order
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        /// <summary>
        /// gets the confirmed order to be deleted.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>RedirectToAction("Index")</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            //finds order 
            Order order = await db.Orders.FindAsync(id);
            
            //removes the order from the database and saves the changes
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        // POST: Orders/Delete/5
        /// <summary>
        /// Deletes the confirmed order.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>RedirectToAction("MyOrders")</returns>
        [HttpPost, ActionName("DeleteMO")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteMOConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            //removes the order from the database
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            return RedirectToAction("MyOrders");
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



        //ApplicationDbContext storeDB = new ApplicationDbContext();
        //AppConfigurations appConfig = new AppConfigurations();


        /// <summary>
        /// retuns the View for Pickupu
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Pickup()
        {
            return View();
        }


        /// <summary>
        /// Creates a Complete Order for Pickup
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>RedirectToAction("Complete"),View(corder)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Pickup(FormCollection values)
        {

            var corder = new CompleteOrders();
            TryUpdateModel(corder);
            var customer = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
            var cart = ShoppingCart.GetCart(this.HttpContext);

            try
            {
                //creates a complete order
                corder.CDeliveryDate = DateTime.Now;

                corder.CDeliveryMethod = "Pick Up";
                corder.CFirstName = customer.FirstName;
                corder.CLastName = customer.LastName;
                corder.CPostalCode = customer.PostalCode;
                corder.CState = customer.State;
                corder.CCity = customer.City;
                corder.CEmail = customer.Email;
                corder.CCountry = customer.Country;
                corder.CPhone = customer.PhoneNumber;
                corder.CAddress = customer.Address;
                corder.CHasPaid = true;
                corder.CUsername = customer.Email;
                corder.COrderDate = DateTime.Now;
                var currentUserId = User.Identity.GetUserId();
                corder.CTotal = cart.GetTotal();
                

                if (corder.CSaveInfo && !corder.CUsername.Equals("guest@guest.com"))
                {

                    var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                    var ctx = store.Context;
                    var currentUser = manager.FindById(User.Identity.GetUserId());



                    //Save this back
                    //http://stackoverflow.com/questions/20444022/updating-user-data-asp-net-identity
                    //var result = await UserManager.UpdateAsync(currentUser);
                    await ctx.SaveChangesAsync();

                    await storeDB.SaveChangesAsync();
                }


                //Save Order
                storeDB.CompleteOrders.Add(corder);
                await storeDB.SaveChangesAsync();
                //Process the order
                cart = ShoppingCart.GetCart(this.HttpContext);
                corder.CTotal = cart.GetTotal();

                cart.EmptyCart();
                try
                {
                    //sends the user an email
  var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                    //Email address to recieve Email
                message.To.Add(new MailAddress(User.Identity.Name)); 
                message.From = new MailAddress("SpeedoModels@outlook.com");
                message.Subject = "Speedo Models Pickup confirmation";
                message.Body = "Thank you, you can pick up your order in store";
                message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        //Email address to send email
                        UserName = "SpeedoModels@outlook.com",  
                        Password = "IrnBru32"  // 
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp-mail.outlook.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                }

                return RedirectToAction("Complete");
                }

                catch
                {
                    return RedirectToAction("Complete");

                }

            

            }
            catch
            {
                //Invalid - redisplay with errors
                return View(corder);
            }
        }



        // GET: Orders/FirstClassCreate
        /// <summary>
        /// Returns View for FirstClassCreate.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult FirstClassCreate()
        {
            return View();
        }






        /// <summary>
        /// Creates a firstclass order.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>RedirectToAction("FirstClass", "Checkouts"),View(order)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> FirstClassCreate(FormCollection values)
        {

            var order = new Order();
            TryUpdateModel(order);
            var customer = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
            var cart = ShoppingCart.GetCart(this.HttpContext);

      

            try
            {
                //Creates an order
                order.DeliveryDate = DateTime.Now.AddDays(1);
                order.DeliveryMethod = "First Class";
                order.FirstName = customer.FirstName;
                order.LastName = customer.LastName;
                order.PostalCode = customer.PostalCode;
                order.State = customer.State;
                order.City = customer.City;
                order.Email = customer.Email; 
                order.Country = customer.Country;
                order.Phone = customer.PhoneNumber;
                order.Address = customer.Address;

                order.Username = customer.Email;
                order.OrderDate = DateTime.Now;
                var currentUserId = User.Identity.GetUserId();
                order.Total = cart.GetFirstClass();

                if (order.SaveInfo && !order.Username.Equals("guest@guest.com"))
                {

                    var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                    var ctx = store.Context;
                    var currentUser = manager.FindById(User.Identity.GetUserId());



                    //Save this back
                    //http://stackoverflow.com/questions/20444022/updating-user-data-asp-net-identity
                    //var result = await UserManager.UpdateAsync(currentUser);
                    await ctx.SaveChangesAsync();

                    await storeDB.SaveChangesAsync();
                }

                //Save Order
                storeDB.Orders.Add(order);
                await storeDB.SaveChangesAsync();
                //Process the order
                cart = ShoppingCart.GetCart(this.HttpContext);
                order.Total = cart.GetFirstClass();
                order = cart.CreateOrder(order);



                return RedirectToAction("FirstClass", "Checkouts");

                

            }
            catch
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }


        // GET: Orders/Create
        /// <summary>
        /// retuns the SecondClassCreate View.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult SecondClassCreate()
        {
            return View();
        }


        /// <summary>
        /// create a second class order.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>return RedirectToAction("SecondClass", "Checkouts"), View(order)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> SecondClassCreate(FormCollection values)
        {

            var order = new Order();
            TryUpdateModel(order);
            var customer = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
            var cart = ShoppingCart.GetCart(this.HttpContext);


            try
            {
                //Creates an order
                order.DeliveryDate = DateTime.Now.AddDays(3);

                order.DeliveryMethod = "Second Class";
                order.FirstName = customer.FirstName;
                order.LastName = customer.LastName;
                order.PostalCode = customer.PostalCode;
                order.State = customer.State;
                order.City = customer.City;
                order.Email = customer.Email;
                order.Country = customer.Country;
                order.Phone = customer.PhoneNumber;
                order.Address = customer.Address;
                //order.timeleft = DateTime.Now;
                order.Username = customer.Email;
                order.OrderDate = DateTime.Now;
                var currentUserId = User.Identity.GetUserId();
                order.Total = cart.GetSecondClass();

                if (order.SaveInfo && !order.Username.Equals("guest@guest.com"))
                {

                    var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                    var ctx = store.Context;
                    var currentUser = manager.FindById(User.Identity.GetUserId());



                    //Save this back
                    //http://stackoverflow.com/questions/20444022/updating-user-data-asp-net-identity
                    //var result = await UserManager.UpdateAsync(currentUser);
                    await ctx.SaveChangesAsync();

                    await storeDB.SaveChangesAsync();
                }


                //Save Order
                storeDB.Orders.Add(order);
                await storeDB.SaveChangesAsync();
                //Process the order
                cart = ShoppingCart.GetCart(this.HttpContext);
                order.Total = cart.GetSecondClass();

                order = cart.CreateOrder(order);


                return RedirectToAction("SecondClass", "Checkouts");
                
               

            }
            catch
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }

        // GET: Orders/CourierCreate
        /// <summary>
        /// retuns the CourierCreate View
        /// </summary>
        /// <returns>View</returns>
        public ActionResult CourierCreate()
        {
            return View();
        }

        // GET: Orders/Create
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns> RedirectToAction("Index", "Products"), RedirectToAction("ToomanyItems", "ShoppingCart"),RedirectToAction("minimum", "ShoppingCart"),View()</returns>
        public ActionResult Create()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            int c = cart.GetCount();
            decimal t = cart.GetTotal();
            //checks if there is less than 1 item in the users cart
            if (c < 1)
            {
                return RedirectToAction("Index", "Products");

            }
            //checks if there is more than 10 items in the users cart

            else if (c >10)
            {
                return RedirectToAction("ToomanyItems", "ShoppingCart");
            }
            //Checks if the total of the users cart is less than 5
            else if(t <5)
            {
                return RedirectToAction("minimum", "ShoppingCart");

            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Couriers the create.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>RedirectToAction("Courier", "Checkouts"), View(order)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> CourierCreate(FormCollection values)
        {

            var order = new Order();
            TryUpdateModel(order);
            var customer = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
            var cart = ShoppingCart.GetCart(this.HttpContext);

           
            try
            {
                //Creates an order

                order.DeliveryDate = DateTime.Now;

                order.DeliveryMethod = "Courier";
                order.FirstName = customer.FirstName;
                order.LastName = customer.LastName;
                order.PostalCode = customer.PostalCode;
                order.State = customer.State;
                order.City = customer.City;
                order.Email = customer.Email;
                order.Country = customer.Country;
                order.Phone = customer.PhoneNumber;
                order.Address = customer.Address;
                //order.timeleft = DateTime.Now;

                order.Username = customer.Email;
                order.OrderDate = DateTime.Now;
                var currentUserId = User.Identity.GetUserId();
                order.Total = cart.GetCourier();

                if (order.SaveInfo && !order.Username.Equals("guest@guest.com"))
                {

                    var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                    var ctx = store.Context;
                    var currentUser = manager.FindById(User.Identity.GetUserId());



                    //Save this back
                    //http://stackoverflow.com/questions/20444022/updating-user-data-asp-net-identity
                    //var result = await UserManager.UpdateAsync(currentUser);
                    await ctx.SaveChangesAsync();

                    await storeDB.SaveChangesAsync();
                }


                //Save Order
                storeDB.Orders.Add(order);
                await storeDB.SaveChangesAsync();
                //Process the order
                cart = ShoppingCart.GetCart(this.HttpContext);
                order.Total = cart.GetCourier();

                order = cart.CreateOrder(order);


                return RedirectToAction("Courier", "Checkouts");

               

            }
            catch
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }

    }
}
