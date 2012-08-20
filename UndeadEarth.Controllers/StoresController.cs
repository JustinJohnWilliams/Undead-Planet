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
    public class StoresController : BaseController
    {
        private IStoreRetriever _storeRetriever;
        private IUserRetriever _userRetriever;

        public StoresController()
        {
            _storeRetriever = DependancyInjection.Instance.Resolve<IStoreRetriever>();
            _userRetriever = DependancyInjection.Instance.Resolve<IUserRetriever>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ActionName("Index")]
        [NoCache]
        [HttpGet]
        public ActionResult Index(double latitude, double longitude, double radius)
        {
            List<StoreNode> storeNodes = new List<StoreNode>();
            //List<IStore> stores = _storeRetriever.GetAllStores();
            List<IStore> stores = _storeRetriever.GetAllStoresInRadius(latitude, longitude, radius);

            storeNodes = stores.Select(i => new StoreNode
            {
                Id = i.Id,
                Latitude = i.Latitude,
                Longitude = i.Longitude,
                Name = i.Name
            }).ToList();

            return Json(storeNodes, JsonRequestBehavior.AllowGet);
        }

        [ActionName("InHotZone")]
        [NoCache]
        [HttpGet]
        public ActionResult InHotZone(Guid userId)
        {
            Guid hotZoneId = _userRetriever.GetUserById(userId).ZoneId;
            List<StoreNode> storeNodes = new List<StoreNode>();
            List<IStore> stores = _storeRetriever.GetAllStoresByHotZoneId(hotZoneId);

            storeNodes = stores.Select(i => new StoreNode
            {
                Id = i.Id,
                Latitude = i.Latitude,
                Longitude = i.Longitude,
                Name = i.Name
            }).ToList();

            return Json(storeNodes, JsonRequestBehavior.AllowGet);
        }

        [ActionName("Items")]
        [NoCache]
        [HttpGet]
        public ActionResult Items(Guid storeId)
        {
            List<IItem> userItems = _storeRetriever.GetItems(storeId);
            List<Item> items = new List<Item>();

            items = userItems.Select(c => new Item
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price,
                IsOneTimeUse = c.IsOneTimeUse
            }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

    }
}
