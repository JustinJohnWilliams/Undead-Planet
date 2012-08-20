using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UndeadEarth.Contract;
using UndeadEarth.Model.Proxy;
using UndeadEarth.Controllers.Models;
using UndeadEarth.Controllers;

namespace UndeadEarth.Web.Controllers
{
    public class SafeHousesController : BaseController
    {
        private ISafeHouseRetriever _safeHouseRetriever;
        private ISafeHouseDirector _safeHouseDirector;
        private IUserRetriever _userRetriever;

        public SafeHousesController()
        {
            _safeHouseRetriever = DependancyInjection.Instance.Resolve<ISafeHouseRetriever>();
            _safeHouseDirector = DependancyInjection.Instance.Resolve<ISafeHouseDirector>();
            _userRetriever = DependancyInjection.Instance.Resolve<IUserRetriever>();
        }

        /// <summary>
        /// Retrieves list of visible safe houses and returns them back to the view
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        [ActionName("Index")]
        [NoCache]
        [HttpGet]
        public ActionResult Index(double latitude, double longitude, double radius)
        {
            List<ISafeHouse> safeHouses = _safeHouseRetriever.GetAllSafeHousesInRadius(latitude, longitude, radius);

            List<SafeHouseNode> safeHouseNodes = new List<SafeHouseNode>();

            safeHouseNodes = safeHouses.Select(i => new SafeHouseNode
            {
                Id = i.Id,
                Latitude = i.Latitude,
                Longitude = i.Longitude
            }).ToList();

            return Json(safeHouseNodes, JsonRequestBehavior.AllowGet);
        }

        [ActionName("InHotZone")]
        [NoCache]
        [HttpGet]
        public ActionResult InHotZone(Guid userId)
        {
            Guid hotzoneId = _userRetriever.GetUserById(userId).ZoneId;
            List<ISafeHouse> safeHouses = _safeHouseRetriever.GetAllSafeHousesByHotZoneId(hotzoneId);

            List<SafeHouseNode> safeHouseNodes = new List<SafeHouseNode>();

            safeHouseNodes = safeHouses.Select(i => new SafeHouseNode
            {
                Id = i.Id,
                Latitude = i.Latitude,
                Longitude = i.Longitude
            }).ToList();

            return Json(safeHouseNodes, JsonRequestBehavior.AllowGet);
        }

        [ActionName("Items")]
        [HttpGet]
        [NoCache]
        public ActionResult Items(Guid safeHouseId, Guid userId)
        {
            List<IItem> safeHouseItems = _safeHouseRetriever.GetItems(userId);

            List<Item> items = safeHouseItems.Select(i => new Item
            {
                Description = i.Description,
                Id = i.Id,
                Name = i.Name,
                Price = i.Price,
                IsOneTimeUse = i.IsOneTimeUse
            }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [ActionName("StoreItem")]
        [HttpPost]
        [NoCache]
        public ActionResult StoreItem(Guid userId, Guid safeHouseId, Guid itemId)
        {
            _safeHouseDirector.TransferItemFromUserToSafeHouse(userId, safeHouseId, itemId);

            return Json(new EmptyResult { });
        }

        [ActionName("UserRetrieveItem")]
        [HttpPost]
        [NoCache]
        public ActionResult UserRetrieveItem(Guid userId, Guid safeHouseId, Guid itemId)
        {
            _safeHouseDirector.TransferItemFromSafeHouseToUser(userId, safeHouseId, itemId);

            return Json(new EmptyResult { });
        }


    }
}
