using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SpeedoModels.Models
{
    /// <summary>
    /// getters and setters for CompleteOrders
    /// </summary>
    [Bind(Exclude = "COrderId")]

    public class CompleteOrders
    {
        public string transactionId { get; set; }


        [ScaffoldColumn(false)]
        [Key]
        public int COrderId { get; set; }

        [ScaffoldColumn(false)]
        public System.DateTime COrderDate { get; set; }

        [ScaffoldColumn(false)]
        public string CUsername { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DisplayName("First Name")]
        [StringLength(160)]
        public string CFirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DisplayName("Last Name")]
        [StringLength(160)]
        public string CLastName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(70)]
        public string CAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(40)]
        public string CCity { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(40)]
        public string CState { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [DisplayName("Postal Code")]
        [StringLength(10)]
        public string CPostalCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(40)]
        public string CCountry { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(24)]
        public string CPhone { get; set; }


        public bool CSaveInfo { get; set; }


        [DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string CEmail { get; set; }


        public string CDeliveryMethod { get; set; }
        public DateTime CDeliveryDate { get; set; }
        public bool CHasPaid { get; set; }
        [ScaffoldColumn(false)]
        public decimal CTotal { get; set; }




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
    /// getters and setters for completeOrderDetails
    /// </summary>
    public class COrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }


    }

}
