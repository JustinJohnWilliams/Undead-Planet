using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using UndeadEarth.Contract;
using System.Web.Security;
using UndeadEarth.Controllers.ViewModels;

namespace UndeadEarth.Controllers
{
    public class FacebookController : BaseController
    {
        private IFacebookGraph _facebookGraph;
        private IHotZoneRetriever _hotZoneRetriever;
        private IUserRetriever _userRetriever;
        private IUserCreationService _userCreationService;

        /// <summary>
        /// Initializes a new instance of the FacebookController class.
        /// </summary>
        public FacebookController()
        {
            _facebookGraph = DependancyInjection.Instance.Resolve<IFacebookGraph>();
            _userRetriever = DependancyInjection.Instance.Resolve<IUserRetriever>();
            _userCreationService = DependancyInjection.Instance.Resolve<IUserCreationService>();
            _hotZoneRetriever = DependancyInjection.Instance.Resolve<IHotZoneRetriever>();
        }

        public static string FacebookAccessToken
        {
            get
            {
                if (global::System.Web.HttpContext.Current.Session["FacebookAccessToken"] == null)
                {
                    global::System.Web.HttpContext.Current.Session["FacebookAccessToken"] = string.Empty;
                }

                return global::System.Web.HttpContext.Current.Session["FacebookAccessToken"] as string;
            }
            set
            {
                global::System.Web.HttpContext.Current.Session["FacebookAccessToken"] = value;
            }
        }

        public static long FacebookUserId
        {
            get
            {
                if (global::System.Web.HttpContext.Current.Session["FacebookUserId"] == null)
                {
                    global::System.Web.HttpContext.Current.Session["FacebookUserId"] = Convert.ToInt64(0);
                }

                return Convert.ToInt64(global::System.Web.HttpContext.Current.Session["FacebookUserId"]);
            }
            set
            {
                global::System.Web.HttpContext.Current.Session["FacebookUserId"] = value;
            }
        }

        [HttpGet]
        [ActionName("Register")]
        public ActionResult Register()
        {
            if (FacebookUserId == 0)
            {
                return RedirectToAction("RedirectToFacebookLogin", "Home");
            }

            if (_userRetriever.FacebookUserExists(FacebookUserId))
            {
                return RedirectToAction("Game", "Home");
            }

            List<KeyValuePair<Guid, string>> startingHotZones = _hotZoneRetriever.GetStartingHotZones();

            StartingHotZoneViewModel viewModel = new StartingHotZoneViewModel
            {
                HotZones = new SelectList(startingHotZones.OrderBy(s => s.Value), "Key", "Value")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ActionName("Register")]
        public ActionResult Register(Guid startingHotZoneId)
        {
            string name = _facebookGraph.GetUser(FacebookAccessToken).Name;
            
            _userCreationService.CreateUser(FacebookUserId, name, startingHotZoneId);

            return RedirectToAction("Game", "Home");
        }
        
        [HttpPost]
        [ActionName("AddFacebookFriend")]
        public ActionResult AddFacebookFriend()
        {
            return RedirectToAction("Invite", new { success = true });
        }

        [HttpGet]
        [ActionName("Invite")]
        public ActionResult Invite(bool? success)
        {
            if(FacebookUserId == 0)
            {
                return RedirectToAction("Index", "Game");
            }

            ViewData["Success"] = success ?? false;

            return View();
        }
    }
}