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
    public class ZombiePacksController : BaseController
    {
        private IZombiePackRetriever _zombiePackRetriever;
        private IUserZombiePackProgressSaver _userZombiePackProgressUpdater;
        private IUserZombiePackProgressRetriever _userZombiePackProgressRetriever;
        private IUserSaver _userSaver;
        private IHuntDirector _huntDirector;
        private IZombiePackDifficultyDirector _zombiePackDifficultyDirector;
        private IUserRetriever _userRetriever;

        public ZombiePacksController()
        {
            _zombiePackRetriever = DependancyInjection.Instance.Resolve<IZombiePackRetriever>();
            _userZombiePackProgressUpdater = DependancyInjection.Instance.Resolve<IUserZombiePackProgressSaver>();
            _userZombiePackProgressRetriever = DependancyInjection.Instance.Resolve<IUserZombiePackProgressRetriever>();
            _userSaver = DependancyInjection.Instance.Resolve<IUserSaver>();
            _huntDirector = DependancyInjection.Instance.Resolve<IHuntDirector>();
            _zombiePackDifficultyDirector = DependancyInjection.Instance.Resolve<IZombiePackDifficultyDirector>();
            _userRetriever = DependancyInjection.Instance.Resolve<IUserRetriever>();
        }

        [ActionName("Index")]
        [HttpGet]
        [NoCache]
        public ActionResult Index(Guid? userId, double? latitude, double? longitude, double? radius)
        {
            List<IZombiePack> zombiePacks = _zombiePackRetriever.GetAllZombiePacksInRadius(latitude.Value, longitude.Value, Convert.ToInt32(radius.Value));

            List<ZombiePackNode> nodes = zombiePacks.Select(i => new ZombiePackNode
            {
                Id = i.Id,
                Latitude = i.Latitude,
                Longitude = i.Longitude,
                Name = i.Name,
                HotZoneId = i.HotZoneId
            }).ToList();

            List<Guid> nodesToRemove = new List<Guid>();
            foreach (ZombiePackNode node in nodes)
            {
                if (_userZombiePackProgressRetriever.IsZombiePackDestroyed(userId.Value, node.Id))
                {
                    nodesToRemove.Add(node.Id);
                }
            }

            foreach (Guid id in nodesToRemove)
            {
                nodes.Remove(nodes.Single(c => c.Id == id));
            }

            return Json(nodes, JsonRequestBehavior.AllowGet);
        }

        [ActionName("InHotzone")]
        [HttpGet]
        [NoCache]
        public ActionResult InHotzone(Guid userId)
        {
            Guid hotzoneId = _userRetriever.GetUserById(userId).ZoneId;
            
            List<IZombiePack> zombiePacks = _zombiePackRetriever.GetAllZombiePacksInHotZone(hotzoneId);

            List<ZombiePackNode> nodes = zombiePacks.Select(i => new ZombiePackNode
            {
                Id = i.Id,
                Latitude = i.Latitude,
                Longitude = i.Longitude,
                Name = i.Name,
                HotZoneId = i.HotZoneId
            }).ToList();

            List<Guid> nodesToRemove = new List<Guid>();
            foreach (ZombiePackNode node in nodes)
            {
                if (_userZombiePackProgressRetriever.IsZombiePackDestroyed(userId, node.Id))
                {
                    nodesToRemove.Add(node.Id);
                }
            }

            foreach (Guid id in nodesToRemove)
            {
                nodes.Remove(nodes.Single(c => c.Id == id));
            }

            return Json(nodes, JsonRequestBehavior.AllowGet);
        }

        [ActionName("Hunt")]
        [HttpPost]
        [NoCache]
        public ActionResult Hunt(Guid userId, Guid zombiePackId)
        {
            _huntDirector.Hunt(userId, zombiePackId);

            return new EmptyResult();
        }

        [ActionName("IsCleared")]
        [HttpGet]
        [NoCache]
        public ActionResult IsCleared(Guid userId, Guid zombiePackId)
        {
            bool isCleared = _userZombiePackProgressRetriever.IsZombiePackDestroyed(userId, zombiePackId);
            return Json(new BooleanResult { Value = isCleared }, JsonRequestBehavior.AllowGet);
        }

        [ActionName("DestructionProgress")]
        [HttpGet]
        [NoCache]
        public ActionResult DestructionProgress(Guid userId, Guid zombiePackId)
        {
            IUserZombiePackProgress progress = _zombiePackDifficultyDirector.GetCalculatedZombiePackProgress(userId, zombiePackId);
            ZombiePackProgress progressResult = new ZombiePackProgress { ZombiesLeft = progress.ZombiesLeft, MaxZombies = progress.MaxZombies };
            progressResult.CostPerHunt = _zombiePackDifficultyDirector.GetEnergyCost(userId, zombiePackId);
            return Json(progressResult, JsonRequestBehavior.AllowGet);
        }
    }
}
