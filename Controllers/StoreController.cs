using SpeedoModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpeedoModels.Controllers
{
    
        // GET: Store
        public class StoreController : Controller
        {
        /// <summary>
        /// The store database
        /// </summary>
        ApplicationDbContext storeDB = new ApplicationDbContext();



        //
        // GET: /Store/Details/5
        /// <summary>
        /// Detailses the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>View(item)</returns>
        public ActionResult Details(int id)
            {
                var item = storeDB.Products.Find(id);

                return View(item);
            }





    }
}