using SpeedoModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpeedoModels.Models
{
    /// <summary>
    /// Getters and Setters for cart
    /// </summary>
    public class Cart
    {
        [Key]
        public int ID { get; set; }
        public string CartId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public System.DateTime DateCreated { get; set; }
        public virtual Product Product { get; set; }
        public decimal CPPrice { get; set; }
        public string CPName { get; set; }
        public decimal Delivery { get; set; }





    }
    /// <summary>
    /// Creates a cartId
    /// </summary>
    public partial class ShoppingCart
    {
        ApplicationDbContext storeDB = new ApplicationDbContext();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        /// <summary>
        /// Gets the cart.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// cart
        /// </returns>
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }
        /// <summary>
        /// Helper method to simplify shopping cart calls
        /// </summary>
        /// <param name="controller"></param>
        /// <returns>GetCart</returns>

        
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        /// <summary>
        /// Adds a product to the cart
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns>
        /// cartItem.Count
        /// </returns>

        public int AddToCart(Product product)
        {
         
           

                // Get the matching cart and item instances
                var cartItem = storeDB.Carts.SingleOrDefault(
                    c => c.CartId == ShoppingCartId
                    && c.ProductId == product.productId);


            

            if (cartItem == null)
                {
                    // Create a new cart item if no cart item exists
                    cartItem = new Cart
                    {
                        ProductId = product.productId,
                        
                        CartId = ShoppingCartId,
                        Count = 1,
                        DateCreated = DateTime.Now,
                        CPName = product.productName,
                        CPPrice = product.Price

                    };
                    storeDB.Carts.Add(cartItem);
                //1 will be taken from the total product stock
                //product.productStock = product.productStock - 1;

              
            }

                else
                {
                // If the item does exist in the cart, 
                // then add one to the quantity
                    cartItem.Count++;
                    //1 will be taken from the total product stock
                    //product.productStock = product.productStock - 1;

                }
                // Save changes
                storeDB.SaveChanges();

                return cartItem.Count;
       
            
        }
        /// <summary>
        /// Removes an item that exists in the shopping cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public int RemoveFromCart(int id)
        {


            // Get the cart

            var cartItem = storeDB.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.ProductId == id);


            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    
                    storeDB.Carts.Remove(cartItem);
                }
                // Save changes
                storeDB.SaveChanges();
                
            }
            return itemCount;
            
        }
        /// <summary>
        /// Removes all items from the cart
        /// </summary>

        public void EmptyCart()
        {
            var cartItems = storeDB.Carts.Where(
                cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                storeDB.Carts.Remove(cartItem);
            }
            // Save changes
            storeDB.SaveChanges();
        }

        /// <summary>
        /// Gets all the items in a cart and returns them in a list
        /// </summary>
        /// <returns>  cart => cart.CartId == ShoppingCartId).ToList();</returns>

        public List<Cart> GetCartItems()
        {
            return storeDB.Carts.Where(
                cart => cart.CartId == ShoppingCartId).ToList();
        }

        /// <summary>
        /// Gets the number of items in a cart
        /// </summary>
        /// <returns>count</returns>

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in storeDB.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }

        /// <summary>
        /// gets the total price of all the items in the cart 
        /// </summary>
        /// <returns>total</returns>

        public decimal GetTotal()
        {
            // Multiply item price by count of that item to get 
            // the current price for each of those items in the cart
            // sum all item price totals to get the cart total
            decimal? total = (from cartItems in storeDB.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count *
                              cartItems.CPPrice).Sum();
            //total = total + Delivery;
            return total ?? decimal.Zero;
        }



        /// <summary>
        /// Gets the total of the items in the cart and adds the fee for first class delivery
        /// </summary>
        /// <returns>
        /// total
        /// </returns>


        public decimal GetFirstClass()
        {
            // Multiply item price by count of that item to get 
            // the current price for each of those items in the cart
            // sum all item price totals to get the cart total
            decimal? total = (from cartItems in storeDB.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?) 5 + cartItems.Count *
                              cartItems.CPPrice).Sum();
            //total = total + Delivery;
            return total ?? decimal.Zero;
        }
        /// <summary>
        /// Gets the total of the items in the cart and adds the fee for second class delivery
        /// </summary>
        /// <returns>
        /// total
        /// </returns>
        public decimal GetSecondClass()
        {
            // Multiply item price by count of that item to get 
            // the current price for each of those items in the cart
            // sum all item price totals to get the cart total
            decimal? total = (from cartItems in storeDB.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?) 2 + cartItems.Count *
                              cartItems.CPPrice).Sum();
            //total = total + Delivery;
            return total ?? decimal.Zero;
        }

        /// <summary>
        /// Gets the total of the items in the cart and adds the fee for a Courier delivery
        /// </summary>
        /// <returns>total</returns>

        public decimal GetCourier()
        {
            // Multiply item price by count of that item to get 
            // the current price for each of those items in the cart
            // sum all item price totals to get the cart total
            decimal? total = (from cartItems in storeDB.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?) 10 + cartItems.Count *
                              cartItems.CPPrice).Sum();
            //total = total + Delivery;
            return total ?? decimal.Zero;
        }
        
        /// <summary>
        /// Creates an order
        /// </summary>
        /// <param name="order"></param>
        /// <returns>order</returns>

        public Order CreateOrder(Order order)
        {
            decimal orderTotal = 0;
            order.OrderDetails = new List<OrderDetail>();

            var cartItems = GetCartItems();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ItemId = item.ProductId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Count
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.Product.Price);
                order.OrderDetails.Add(orderDetail);
                storeDB.OrderDetails.Add(orderDetail);

            }
            // Set the order's total to the orderTotal count
            order.Total = orderTotal;

            // Save the order
            storeDB.SaveChanges();
            
            // Return the OrderId as the confirmation number
            return order;
        }

        /// <summary>
        /// Gets the cart Id
        /// </summary>
        /// <param name="context"></param>
        /// <returns>context.Session[CartSessionKey].ToString();</returns>
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] =
                        context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }


        /// <summary>
        ///  When a user has logged in, migrate their shopping cart to be associated with their username
        /// </summary>
        /// <param name="userName"></param>
        public void MigrateCart(string userName)
        {
            var shoppingCart = storeDB.Carts.Where(
                c => c.CartId == ShoppingCartId);

            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            storeDB.SaveChanges();
        }
    }
}