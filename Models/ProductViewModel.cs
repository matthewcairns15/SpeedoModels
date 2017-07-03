using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpeedoModels.Models
{
    /// <summary>
    /// View Model for Product
    /// </summary>
    public class ProductViewModel
    {

        public IEnumerable<Product> Products { get; set; }
    }
}  