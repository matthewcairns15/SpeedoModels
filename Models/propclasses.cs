using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SpeedoModels.Models
{
    /// <summary>
    /// Getters and Setters for Refunds
    /// </summary>
    public class Refunds
    {
        [Key]
        public int RefundId { get; set; }
        public string transactionId { get; set; }
        public string UserEmail { get; set; }
        public decimal RefundTotal { get; set; }
    }

    /// <summary>
    /// Getters and Setters for Customer
    /// </summary>
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string country { get; set; }
        public string address { get; set; }
        public string TelephoneNumber { get; set; }


      
    }




    /// <summary>
    /// Getters and Setters for Order
    /// </summary>


    [Bind(Exclude = "OrderId")]
    public  class Order
    {


        [ScaffoldColumn(false)]
        public int OrderId { get; set; }

        [ScaffoldColumn(false)]
        public System.DateTime OrderDate { get; set; }

        [ScaffoldColumn(false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DisplayName("First Name")]
        [StringLength(160)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DisplayName("Last Name")]
        [StringLength(160)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(70)]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(40)]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(40)]
        public string State { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [DisplayName("Postal Code")]
        [StringLength(10)]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(40)]
        public string Country { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(24)]
        public string Phone { get; set; }


        public bool SaveInfo { get; set; }


        [DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        public string DeliveryMethod { get; set; }
        public DateTime DeliveryDate { get; set; }
        //public DateTime timeleft { get; set; }
        public bool HasPaid { get; set; }
        [ScaffoldColumn(false)]
        public decimal Total { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }




        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(Order order)
        {
            System.Text.StringBuilder bob = new StringBuilder();

            bob.Append("<p>Order Information for Order: " + order.OrderId + "<br>Placed at: " + order.OrderDate + "</p>").AppendLine();
            bob.Append("<p>Name: " + order.FirstName + " " + order.LastName + "<br>");
            bob.Append("Address: " + order.Address + " " + order.City + " " + order.State + " " + order.PostalCode + "<br>");
            bob.Append("Contact: " + order.Email + "     " + order.Phone + "</p>");

            bob.Append("<br>").AppendLine();
            bob.Append("<Table>").AppendLine();
            // Display header 
            string header = "<tr> <th>Item Name</th>" + "<th>Quantity</th>" + "<th>Price</th> <th></th> </tr>";
            bob.Append(header).AppendLine();

            String output = String.Empty;
            try
            {
                foreach (var item in order.OrderDetails)
                {
                    bob.Append("<tr>");
                    output = "<td>" + item.Product.productName + "</td>" + "<td>" + item.Quantity + "</td>" + "<td>" + item.Quantity * item.UnitPrice + "</td>";
                    bob.Append(output).AppendLine();
                    Console.WriteLine(output);
                    bob.Append("</tr>");
                }
            }
            catch (Exception ex)
            {
                output = "No items ordered.";
            }
            bob.Append("</Table>");
            bob.Append("<b>");
            // Display footer 
            string footer = String.Format("{0,-12}{1,12}\n",
                                          "Total", order.Total);
            bob.Append(footer).AppendLine();
            bob.Append("</b>");

            return bob.ToString();
        }
    }

    /// <summary>
    /// Getters and Setters for OrderDetails
    /// </summary>

    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }


    }

    /// <summary>
    /// Getters and Setters for Product
    /// </summary>
    public class Product
    {
        
        [Required]
        public int productId { get; set; }
        [Required]
        public string productName { get; set; }
        [Required]
        public string productDescription { get; set; }
        
        //Validates the price

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999.99, ErrorMessage = "Price must be between 0.01 and 999.99")]
        public decimal Price { get; set; }
        public string productColour { get; set; }
        [Required]
        public int productStock { get; set; }




    }

    











}