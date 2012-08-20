using System;
using System.Web.Mvc;
using System.Configuration;
using UndeadEarth.Controllers.Models;
using UndeadEarth.Controllers;
using UndeadEarth.Contract;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using UndeadEarth.Controllers.ViewModels;

namespace UndeadEarth.Web.Controllers
{
    [HandleError]
    public class HomeController : BaseController
    {
        private IUserRetriever _userRetriever = null;
        private IFacebookGraph _facebookGraph;
        private IUserStatsRetriever _userStatsRetriever;

        public HomeController()
        {
            _userRetriever = DependancyInjection.Instance.Resolve<IUserRetriever>();
            //_facebookGraph = DependancyInjection.Instance.Resolve<IFacebookGraph>();
            _userStatsRetriever = DependancyInjection.Instance.Resolve<IUserStatsRetriever>();
        }

        /// <summary>
        /// Initializes the silverlight control and returns the home page.
        /// </summary>
        /// <returns></returns>
        [NoCache]
        [HttpGet]
        [ActionName("Game")]
        public ActionResult Game()
        {
            ViewData["BaseUri"] = ConfigurationManager.AppSettings["BaseUri"]; //this is the base uri for all silverlight to mvc communication.

            if (IsInSandboxMode())
            {
                ViewData["UserId"] = "AAAFCB18-3BBD-4842-A1D2-8D19D68EF52E";
            }
            else
            {
                long facebookUserId = FacebookController.FacebookUserId;
                if(facebookUserId == 0)
                {
                    return RedirectToAction("RedirectToFacebookLogin");
                }

                Guid userId = _userRetriever.GetUserByFacebookId(facebookUserId).Id;
                ViewData["UserId"] = userId.ToString();
            }

            return View();
        }

        [NoCache]
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Index(string signed_request)
        {
            HomePageViewModel homePageViewModel = new HomePageViewModel();
            homePageViewModel.IsRegistered = false;
            homePageViewModel.UserRank = new List<UserStats>();

            if(string.IsNullOrEmpty(signed_request))
            {
                return RedirectToAction("RedirectToFacebookLogin");
            }

            if(_facebookGraph.ValidateSignedRequest(signed_request))
            {
                if(_facebookGraph.IsLoggedIn(signed_request))
                {
                    FacebookController.FacebookAccessToken = _facebookGraph.GetAccessTokenFromSignedRequest(signed_request);
                    FacebookController.FacebookUserId = _facebookGraph.GetUserFromSignedRequest(signed_request).UserId;
                    if (_userRetriever.FacebookUserExists(FacebookController.FacebookUserId) == false)
                    {
                        return View(homePageViewModel);
                    }
                    else
                    {
                        homePageViewModel.IsRegistered = true;
                        List<IFacebookFriend> friends = _facebookGraph.GetFriends(FacebookController.FacebookAccessToken);
                        List<long> friendIds = new List<long>() { FacebookController.FacebookUserId };
                        friendIds.AddRange(friends.Select(s => s.Id));
                        homePageViewModel.UserRank = _userStatsRetriever.GetUsersRank(friendIds);
                        return View(homePageViewModel);
                    }
                }
                else
                {
                    return RedirectToAction("RedirectToFacebookLogin");
                }
            }

            return RedirectToAction("RedirectToFacebookLogin");
        }

        [NoCache]
        [HttpGet]
        public ActionResult RedirectToFacebookLogin()
        {
            return View();
        }

        private static bool IsInSandboxMode()
        {
            return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Sandbox"]);
        }
    }
}
