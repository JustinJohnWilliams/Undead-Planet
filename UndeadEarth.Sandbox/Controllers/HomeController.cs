using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using UndeadEarth.Web.Models;
using UndeadEarth.Model.Proxy;
using UndeadEarth.Contract;
using UndeadEarth.Model;

namespace UndeadEarth.Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        /// <summary>
        /// Initializes the silverlight control and returns the home page.
        /// </summary>
        /// <returns></returns>
        [NoCache]
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Index()
        {
            ViewData["BaseUri"] = ConfigurationManager.AppSettings["BaseUri"]; //this is the base uri for all silverlight to mvc communication.
            ViewData["UserId"] = "AAAFCB18-3BBD-4842-A1D2-8D19D68EF52E";  //this will be retrieved from the authentication mechanism
            return View();
        }
    }
}
