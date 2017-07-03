using Braintree;
using SpeedoModels.Models;
using SpeedoModels.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpeedoModels.Controllers
{
    
        public class ShoppingCartController : Controller
        {

        /// <summary>
        /// The store database
        /// </summary>
        ApplicationDbContext storeDB = new ApplicationDbContext();

        //
        // GET: /ShoppingCart/
        /// <summary>
        /// retuns the Index View
        /// </summary>
        /// <returns>View(viewModel)</returns>
        public ActionResult Index()
            {
                var cart = ShoppingCart.GetCart(this.HttpContext);
           

                // Set up our ViewModel
                var viewModel = new ShoppingCartViewModel
                {
                    CartItems = cart.GetCartItems(),
                    CartTotal = cart.GetTotal()
                };
                // Return the view
                return View(viewModel);
            }

        /// <summary>
        /// retuns the Indexnocheckouts View
        /// </summary>
        /// <returns>View(viewModel)</returns>
        public ActionResult Indexnocheckout()
        {
            //gets the current cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            // Return the view
            return View(viewModel);
        }


        /// <summary>
        /// retuns the ToomanyItems View
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        public ActionResult ToomanyItems()
        {
            return View();
        }

        /// <summary>
        /// retuns the outofstock view
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        public ActionResult OutofStock()
        {
            return View();
        }

        /// <summary>
        /// retuns the minimum View
        /// </summary>
        /// <returns>
        /// View
        /// </returns>
        public ActionResult minimum()
        {
            return View();
        }


        //
        // GET: /Store/AddToCart/5
        /// <summary>
        /// Adds item to cart.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>View("outofstock"),Json(results)</returns>
        [HttpPost]
            public ActionResult AddToCart(int id)
            {
            // Retrieve the item from the database
            var addedItem = storeDB.Products
                .Single(item => item.productId == id);
           

            //checks if the products stock is less than 1
            if (addedItem.productStock < 1)
            {
                return View("outofstock");
            }

            
            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            addedItem.productStock = addedItem.productStock - 1;

            storeDB.SaveChanges();

            int count = cart.AddToCart(addedItem);

                // Display the confirmation message
                var results = new ShoppingCartRemoveViewModel
                {
                    Message = Server.HtmlEncode(addedItem.productName) +
                        " has been added to your shopping cart.",
                    CartTotal = cart.GetTotal(),
                    CartCount = cart.GetCount(),
                    ItemCount = count,
                    DeleteId = id
                    
                };
                return Json(results);

                // Go back to the main store page for more shopping
                // return RedirectToAction("Index");
            }

        /// <summary>
        /// Gets the VM for firstclass.
        /// </summary>
        /// <returns>View(viewModel)</returns>
        [HttpPost]
        public ActionResult FirstClass()
        {

            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetFirstClass()
            };
            // Return the view
            return View(viewModel);
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        /// <summary>
        /// Removes item from cart.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Json(results)</returns>
        [HttpPost]
            public ActionResult RemoveFromCart(int id)
            {
                // Remove the item from the cart
                var cart = ShoppingCart.GetCart(this.HttpContext);

            
                // Get the name of the item to display confirmation

                // Get the name of the product to display confirmation
                string itemName = storeDB.Products
                    .Single(item => item.productId == id).productName;

                // Remove from cart
                int itemCount = cart.RemoveFromCart(id);


            var removed = storeDB.Products
                        .Single(item => item.productId == id);
            removed.productStock = removed.productStock + 1;

            storeDB.SaveChanges();

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
                {
                    Message = "One (1) " + Server.HtmlEncode(itemName) +
                        " has been removed from your shopping cart.",
                    CartTotal = cart.GetTotal(),
                    CartCount = cart.GetCount(),
                    ItemCount = itemCount,
                    DeleteId = id
                };
      
                return Json(results);
            }
        //
        // GET: /ShoppingCart/CartSummary
        /// <summary>
        ///  gets the Cart summary.
        /// </summary>
        /// <returns>PartialView("CartSummary");</returns>
        [ChildActionOnly]
            public ActionResult CartSummary()
            {
                var cart = ShoppingCart.GetCart(this.HttpContext);

                ViewData["CartCount"] = cart.GetCount();
                return PartialView("CartSummary");
            }










    }
}
