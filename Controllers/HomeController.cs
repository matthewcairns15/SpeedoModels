using SpeedoModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;

namespace SpeedoModels.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// The database
        /// </summary>
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns Index View with products db in list form
        /// </summary>
        /// <returns>View(db.Products.ToList())</returns>
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }



        /// <summary>
        /// Views the model.
        /// </summary>
        /// <returns>View(mymodel)</returns>
        public ActionResult ViewModel()
        {
            ViewBag.Message = "Demo passing multiple models to a view using dynamic models";
            ProductViewModel mymodel = new ProductViewModel();
            //displays the products in a list
            mymodel.Products = db.Products.ToList();


            return View(mymodel);
        }

        /// <summary>
        /// returns the Createorder view.
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult createOrder()
        {
            return View();
        }


        /// <summary>
        /// Views the item.
        /// </summary>
        /// <param name="models">The models.</param>
        /// <returns>RedirectToAction("Details", "Products")</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ViewItem(Product models)
        {
            return RedirectToAction("Details", "Products");
        }

        /// <summary>
        /// Contacts this instance.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Contact()
        {
            return View();
        }
        /// <summary>
        /// Sents this instance.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Sent()
        {
            return View();
        }
       

    }






}


