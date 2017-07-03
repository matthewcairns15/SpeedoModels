using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Mvc;

namespace SpeedoModels.Models
{
  

    /// <summary>
    /// Getters and Setters for the product backup
    /// </summary>



    public class ProductBackup
    {
        [Required]
        [Key]
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
    

    /// <summary>
    /// Getters and Setters for the complete order backup
    /// </summary>


    [Bind(Exclude = "OrderId")]
    public class OrderBackup
    {

        public string transactionId { get; set; }

        [ScaffoldColumn(false)]
        [Key]
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
        public bool HasPaid { get; set; }
        [ScaffoldColumn(false)]
        public decimal Total { get; set; }


        
        
    }

  





}