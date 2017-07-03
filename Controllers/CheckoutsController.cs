using Braintree;
using System;
using System.Linq;
using SpeedoModels.Models;

using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SpeedoModels.Configuration;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace SpeedoModels.Controllers
{
   
    public class CheckoutsController : Controller
    {


        private ApplicationDbContext db = new ApplicationDbContext();

        public IBraintreeConfiguration config = new BraintreeConfiguration();

        /// <summary>
        /// The transaction success statuses
        /// </summary>
        public static readonly TransactionStatus[] transactionSuccessStatuses = {
                                                                                    TransactionStatus.AUTHORIZED,
                                                                                    TransactionStatus.AUTHORIZING,
                                                                                    TransactionStatus.SETTLED,
                                                                                    TransactionStatus.SETTLING,
                                                                                    TransactionStatus.SETTLEMENT_CONFIRMED,
                                                                                    TransactionStatus.SETTLEMENT_PENDING,
                                                                                    TransactionStatus.SUBMITTED_FOR_SETTLEMENT
                                                                                };
        /// <summary>
        /// gets a braintree gateway and generates a clienttoken  
        /// </summary>
        /// <returns>View</returns>
        public ActionResult New()
        {
            var gateway = config.GetGateway();
            var clientToken = gateway.ClientToken.generate();
            ViewBag.ClientToken = clientToken;
            return View();
        }
        /// <summary>
        /// gets a braintree gateway and generates a clienttoken  
        /// </summary>
        /// <returns>View</returns>
        public ActionResult FirstClass()
        {
            var gateway = config.GetGateway();
            var clientToken = gateway.ClientToken.generate();
            ViewBag.ClientToken = clientToken;
            return View();
        }
        /// <summary>
        /// gets a braintree gateway and generates a clienttoken  
        /// </summary>
        /// <returns>View</returns>
        public ActionResult SecondClass()
        {
            var gateway = config.GetGateway();
            var clientToken = gateway.ClientToken.generate();
            ViewBag.ClientToken = clientToken;
            return View();
        }
        /// <summary>
        /// gets a braintree gateway and generates a clienttoken
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        public ActionResult Courier()
        {
            var gateway = config.GetGateway();
            var clientToken = gateway.ClientToken.generate();
            ViewBag.ClientToken = clientToken;
            return View();
        }


        /// <summary>
        /// Creates a complete order after the customer has paid for their order and delivery charge
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// RedirectToAction("New"),RedirectToAction("Show", new { id = transaction.Id }),
        /// </returns>

        public async Task<ActionResult> CreateFirstClass(FormCollection collection)
        {
            var gateway = config.GetGateway();


      

            string nonceFromTheClient = collection["payment_method_nonce"];

            var cart = ShoppingCart.GetCart(this.HttpContext);

           


            var request = new TransactionRequest
            {
                //gets the amount to charge the user
                Amount = cart.GetFirstClass(),
                PaymentMethodNonce = nonceFromTheClient,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };
            //empties the users cart 
            cart.EmptyCart();

            Result<Transaction> result = gateway.Transaction.Sale(request);



            if (result.IsSuccess())
            {
                //Gets the newest order from the database that has the same Email as the current application user and changes
                //HasPaid to true 

                var orderToBeChanged = db.Orders
                  .Where(o => o.HasPaid == false && o.Email == User.Identity.Name)
                        .OrderByDescending((o => o.OrderDate)).First();
                orderToBeChanged.HasPaid = true;
                db.SaveChanges();


                var corders = new CompleteOrders();
                TryUpdateModel(corders);

                //Creates a complete order using the data from the order to be changes


                corders.CAddress = orderToBeChanged.Address;
                corders.CDeliveryMethod = orderToBeChanged.DeliveryMethod;
                corders.CFirstName = orderToBeChanged.FirstName;
                corders.CLastName = orderToBeChanged.LastName;
                corders.CPostalCode = orderToBeChanged.PostalCode;
                corders.CState = orderToBeChanged.State;
                corders.CCity = orderToBeChanged.City;
                corders.CEmail = orderToBeChanged.Email;
                corders.CCountry = orderToBeChanged.Country;
                corders.CPhone = orderToBeChanged.Phone;
                corders.COrderDate = orderToBeChanged.OrderDate;
                corders.CTotal = orderToBeChanged.Total;
                corders.CUsername = orderToBeChanged.Username;
                corders.CHasPaid = orderToBeChanged.HasPaid;
                corders.CDeliveryDate = orderToBeChanged.DeliveryDate;
                corders.CSaveInfo = orderToBeChanged.SaveInfo;


                storeDB.CompleteOrders.Add(corders);
                await storeDB.SaveChangesAsync();


                Transaction transaction = result.Target;

                //Sends the user an email to confirm that their order was created

                try
                {
                    var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                    var message = new MailMessage();
                    //email address to recieve email
                    message.To.Add(new MailAddress(User.Identity.Name));
                    message.From = new MailAddress("SpeedoModels@outlook.com");
                    message.Subject = "Speedo Models Purchase";
                    message.Body = "Thank you for shopping with Speedomodels";
                    message.IsBodyHtml = true;
                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            //Email address to send email details
                            UserName = "SpeedoModels@outlook.com",
                            Password = "IrnBru32"
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp-mail.outlook.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);
                    }





                    return RedirectToAction("Show", new { id = transaction.Id });
                }
                catch
                {
                    return RedirectToAction("Show", new { id = transaction.Id });

                }

            }
            //handles errors for the transaction
            else if (result.Transaction != null)
            {
                return RedirectToAction("Show", new { id = result.Transaction.Id });
            }
            else
            {
                string errorMessages = "";
                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                }
                TempData["Flash"] = errorMessages;
                return RedirectToAction("New");
            }

        }


        /// <summary>
        /// Creates a complete order after the customer has paid for their order and delivery charge
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// RedirectToAction("New"),RedirectToAction("Show", new { id = transaction.Id }),
        /// </returns>

        public async Task<ActionResult> CreateSecondClass(FormCollection collection)
        {
            var gateway = config.GetGateway();



            string nonceFromTheClient = collection["payment_method_nonce"];

            var cart = ShoppingCart.GetCart(this.HttpContext);

           


            var request = new TransactionRequest
            {
                //gets the amount to charge the user
                Amount = cart.GetSecondClass(),
                PaymentMethodNonce = nonceFromTheClient,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };
            cart.EmptyCart();
            //empties shopping cart

            Result<Transaction> result = gateway.Transaction.Sale(request);



            if (result.IsSuccess())
            {
                //Gets the newest order from the database that has the same Email as the current application user and changes
                //HasPaid to true 

                var orderToBeChanged = db.Orders
                  .Where(o => o.HasPaid == false && o.Email == User.Identity.Name)
                        .OrderByDescending((o => o.OrderDate)).First();
                orderToBeChanged.HasPaid = true;
                db.SaveChanges();

                

                    var corders = new CompleteOrders();
                TryUpdateModel(corders);


                //Creates a complete order using the order to be changed details


                corders.CAddress = orderToBeChanged.Address;
                    corders.CDeliveryMethod = orderToBeChanged.DeliveryMethod;
                    corders.CFirstName = orderToBeChanged.FirstName;
                    corders.CLastName = orderToBeChanged.LastName;
                    corders.CPostalCode = orderToBeChanged.PostalCode;
                    corders.CState = orderToBeChanged.State;
                    corders.CCity = orderToBeChanged.City;
                    corders.CEmail = orderToBeChanged.Email;
                    corders.CCountry = orderToBeChanged.Country;
                    corders.CPhone = orderToBeChanged.Phone;
                    corders.COrderDate = orderToBeChanged.OrderDate;
                    corders.CTotal = orderToBeChanged.Total;
                    corders.CUsername = orderToBeChanged.Username;
                    corders.CHasPaid = orderToBeChanged.HasPaid;
                corders.CDeliveryDate = orderToBeChanged.DeliveryDate;
                corders.CSaveInfo = orderToBeChanged.SaveInfo;
                

                    storeDB.CompleteOrders.Add(corders);
                    await storeDB.SaveChangesAsync();
              

                Transaction transaction = result.Target;
                //Sends the user an email to confirm their order

                try
                {

                    var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                    var message = new MailMessage();
                    //Email address to recieve email
                    message.To.Add(new MailAddress(User.Identity.Name));
                    message.From = new MailAddress("SpeedoModels@outlook.com");
                    message.Subject = "Speedo Models Purchase";
                    message.Body = "Thank you for shopping with Speedomodels";
                    message.IsBodyHtml = true;
                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            //Email address to send email

                            UserName = "SpeedoModels@outlook.com",
                            Password = "IrnBru32"
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp-mail.outlook.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);
                    }

                    return RedirectToAction("Show", new { id = transaction.Id });
                }
                catch
                {
                    return RedirectToAction("Show", new { id = transaction.Id });


                }
            }
            //handles errors with the transaction
            else if (result.Transaction != null)
            {
                return RedirectToAction("Show", new { id = result.Transaction.Id });
            }
            else
            {
                string errorMessages = "";
                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                }
                TempData["Flash"] = errorMessages;
                return RedirectToAction("New");
            }

        }


        /// <summary>
        /// Creates a complete order after the customer has paid for their order and delivery charge
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// RedirectToAction("New"),RedirectToAction("Show", new { id = transaction.Id }),
        /// </returns>
        public async Task<ActionResult> CreateCourier(FormCollection collection)
        {
            var gateway = config.GetGateway();
           


            string nonceFromTheClient = collection["payment_method_nonce"];

            var cart = ShoppingCart.GetCart(this.HttpContext);

          

            var request = new TransactionRequest
            {
                //gets amount to charge user
                Amount = cart.GetCourier(),
                PaymentMethodNonce = nonceFromTheClient,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };
            //empties shopping cart
            cart.EmptyCart();

            Result<Transaction> result = gateway.Transaction.Sale(request);



            if (result.IsSuccess())
            {
                //Gets the newest order from the database that has the same Email as the current application user and changes
                var orderToBeChanged = db.Orders
                  .Where(o => o.HasPaid == false && o.Email == User.Identity.Name)
                        .OrderByDescending((o => o.OrderDate)).First();
                orderToBeChanged.HasPaid = true;
                db.SaveChanges();

                var corders = new CompleteOrders();
                TryUpdateModel(corders);

                //Creates a complete order using the order to be changed details



                corders.CAddress = orderToBeChanged.Address;
                corders.CDeliveryMethod = orderToBeChanged.DeliveryMethod;
                corders.CFirstName = orderToBeChanged.FirstName;
                corders.CLastName = orderToBeChanged.LastName;
                corders.CPostalCode = orderToBeChanged.PostalCode;
                corders.CState = orderToBeChanged.State;
                corders.CCity = orderToBeChanged.City;
                corders.CEmail = orderToBeChanged.Email;
                corders.CCountry = orderToBeChanged.Country;
                corders.CPhone = orderToBeChanged.Phone;
                corders.COrderDate = orderToBeChanged.OrderDate;
                corders.CTotal = orderToBeChanged.Total;
                corders.CUsername = orderToBeChanged.Username;
                corders.CHasPaid = orderToBeChanged.HasPaid;
                corders.CDeliveryDate = orderToBeChanged.DeliveryDate;
                corders.CSaveInfo = orderToBeChanged.SaveInfo;


                storeDB.CompleteOrders.Add(corders);
                await storeDB.SaveChangesAsync();


                Transaction transaction = result.Target;

                //sends user an email to confirm order
                try
                {

                    var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                    var message = new MailMessage();
                    //Email address to recieve email 
                    message.To.Add(new MailAddress(User.Identity.Name));
                    message.From = new MailAddress("SpeedoModels@outlook.com");
                    message.Subject = "Speedo Models Purchase";
                    message.Body = "Thank you for shopping with Speedomodels";
                    message.IsBodyHtml = true;
                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            //eamil address to send email
                            UserName = "SpeedoModels@outlook.com",
                            Password = "IrnBru32"
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp-mail.outlook.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);
                    }

                    return RedirectToAction("Show", new { id = transaction.Id });
                }
                catch
                {
                    return RedirectToAction("Show", new { id = transaction.Id });

                }
            }


            //handles errors with the transaction
            else if (result.Transaction != null)
            {
                return RedirectToAction("Show", new { id = result.Transaction.Id });
            }
            else
            {
                string errorMessages = "";
                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                }
                TempData["Flash"] = errorMessages;
                return RedirectToAction("New");
            }

        }

        /// <summary>
        /// Shows the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>View</returns>
        public ActionResult Show(String id)
        {
            var gateway = config.GetGateway();
            Transaction transaction = gateway.Transaction.Find(id);
            //Displays the newest order in the database where the order email equals the users name
            var orderToBeChanged = db.CompleteOrders
                 .Where(o =>  o.CEmail == User.Identity.Name)
                       .OrderByDescending((o => o.COrderDate)).First();
            //sets the transaction Id
            orderToBeChanged.transactionId = transaction.Id;

            db.SaveChanges();

        



            if (transactionSuccessStatuses.Contains(transaction.Status))
            {
                TempData["header"] = "Sweet Success!";
                TempData["icon"] = "success";
                TempData["message"] = "Your test transaction has been successfully processed. See the Braintree API response and try again.";
            }
            else
            {
                TempData["header"] = "Transaction Failed";
                TempData["icon"] = "fail";
                TempData["message"] = "Your test transaction has a status of " + transaction.Status + ". See the Braintree API response and try again.";
            };

            ViewBag.Transaction = transaction;
            return View();
        }




        ApplicationDbContext storeDB = new ApplicationDbContext();
        AppConfigurations appConfig = new AppConfigurations();

        /// <summary>
        /// Creates the order.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateOrder(FormCollection values)
        {

            var order = new Order();
            TryUpdateModel(order);

            try
            {
                //sets order details
                order.Username = User.Identity.Name;
                order.Email = User.Identity.Name;
                order.OrderDate = DateTime.Now;
                var currentUserId = User.Identity.GetUserId();

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
                var cart = ShoppingCart.GetCart(this.HttpContext);
                order = cart.CreateOrder(order);




                return RedirectToAction("Complete",
                    new { id = order.OrderId });

            }
            catch
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }


        private class HttpBasicAuthenticator
        {
            private string emailApiKey;
            private string v;

            /// <summary>
            /// Initializes a new instance of the <see cref="HttpBasicAuthenticator"/> class.
            /// </summary>
            /// <param name="v">The v.</param>
            /// <param name="emailApiKey">The email API key.</param>
            /// 
            public HttpBasicAuthenticator(string v, string emailApiKey)
            {
                this.v = v;
                this.emailApiKey = emailApiKey;
            }
        }


    }
}