using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UndeadEarth.Contract;
using UndeadEarth.Model.Proxy;
using UndeadEarth.Web.Models;

namespace UndeadEarth.Web.Controllers
{
    public class HotZonesController : Controller
    {
        private IHotZoneRetriever _hotZoneRetriever;

        public HotZonesController()
        {
            _hotZoneRetriever = MvcApplication.DependancyInjection.Resolve<IHotZoneRetriever>();
        }

        /// <summary>
        /// Retrieves all hot zones to display on the view.
        /// </summary>
        /// <returns></returns>
        [ActionName("Index")]
        [HttpGet]
        [NoCache]
        public ActionResult Index(Guid userId)
        {
            List<HotZoneNode> nodes = new List<HotZoneNode>();
            List<IHotZone> hotZones = _hotZoneRetriever.GetAllHotZones();

            nodes = hotZones.Select(i => new HotZoneNode
            {
                Id = i.Id,
                Latitude = i.Latitude,
                Longitude = i.Longitude,
                Name = i.Name
            }).ToList();

            return Json(nodes, JsonRequestBehavior.AllowGet);
        }

        [ActionName("ZombiePacksLeft")]
        [HttpGet]
        [NoCache]
        public ActionResult ZombiePacksLeft(Guid userId, Guid hotZoneId)
        {
            int zombiePacksLeft = _hotZoneRetriever.ZombiePacksLeft(userId, hotZoneId);

            return Json(new IntResult { Value = zombiePacksLeft }, JsonRequestBehavior.AllowGet);
        }

        [ActionName("Uncleared")]
        [HttpGet]
        [NoCache]
        public ActionResult Uncleared(Guid userId)
        {
            List<KeyValuePair<Guid, int>> clearedHotZoneIds = _hotZoneRetriever.GetRemainingZombiePacksInHotZones(userId);

            return Json(clearedHotZoneIds, JsonRequestBehavior.AllowGet);
        }

    }
}
