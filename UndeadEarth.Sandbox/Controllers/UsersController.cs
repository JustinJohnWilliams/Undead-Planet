using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UndeadEarth.Contract;
using UndeadEarth.Model;
using UndeadEarth.Model.Proxy;
using UndeadEarth.Web.Models;

namespace UndeadEarth.Web.Controllers
{
    public class UsersController : Controller
    {
        private int _userSpeed = int.MaxValue; //mph

        private IZombiePackRetriever _zombiePackRetriever;
        private IHotZoneRetriever _hotZoneRetriever;
        private IUserRetriever _userRetriever;
        private IUserSaver _userSaver;
        private IUserZombiePackProgressRetriever _userHotZoneProgressRetriever;
        private IUserItemRetriever _userItemRetriever;
        private IUserCountsRetriever _userCountsRetriever;

        private IDistanceCalculator _distanceCalculator;
        private IUserEnergyProvider _userEnergyProvider;
        private IUserMoveDirector _userMover;
        private IShopDirector _shopDirector;
        private IUserAttackPowerProvider _userAttackPowerProvider;
        private IUserSightRadiusProvider _userSightRadiusProvider;
        private IItemUsageDirector _itemUsageDirector;
        private IAchievementProvider _achievementProvider;
        private IUserLevelService _userLevelService;

        public UsersController()
        {
            _zombiePackRetriever = MvcApplication.DependancyInjection.Resolve<IZombiePackRetriever>();
            _hotZoneRetriever = MvcApplication.DependancyInjection.Resolve<IHotZoneRetriever>();
            _userRetriever = MvcApplication.DependancyInjection.Resolve<IUserRetriever>();
            _userSaver = MvcApplication.DependancyInjection.Resolve<IUserSaver>();
            _userHotZoneProgressRetriever = MvcApplication.DependancyInjection.Resolve<IUserZombiePackProgressRetriever>();
            _userItemRetriever = MvcApplication.DependancyInjection.Resolve<IUserItemRetriever>();

            _distanceCalculator = MvcApplication.DependancyInjection.Resolve<IDistanceCalculator>();
            _userEnergyProvider = MvcApplication.DependancyInjection.Resolve<IUserEnergyProvider>();
            _userMover = MvcApplication.DependancyInjection.Resolve<IUserMoveDirector>();
            _shopDirector = MvcApplication.DependancyInjection.Resolve<IShopDirector>();
            _userAttackPowerProvider = MvcApplication.DependancyInjection.Resolve<IUserAttackPowerProvider>();
            _userSightRadiusProvider = MvcApplication.DependancyInjection.Resolve<IUserSightRadiusProvider>();
            _itemUsageDirector = MvcApplication.DependancyInjection.Resolve<IItemUsageDirector>();
            _achievementProvider = MvcApplication.DependancyInjection.Resolve<IAchievementProvider>();
            _userLevelService = MvcApplication.DependancyInjection.Resolve<IUserLevelService>();
            _userCountsRetriever = MvcApplication.DependancyInjection.Resolve<IUserCountsRetriever>();
        }

        /// <summary>
        /// Gets user for view or returns null if user does not exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("Index")]
        [HttpGet]
        [NoCache]
        public ActionResult Index(Guid? userId)
        {
            UserNode userNode = GetUserNode(userId);

            if (userNode != null)
            {
                return Json(userNode, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new object(), JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("Location")]
        [HttpPost]
        [NoCache]
        public ActionResult Location(Guid? userId, double? latitude, double? longitude)
        {
            _userMover.MoveUser(userId.Value, latitude.Value, longitude.Value);
            return new EmptyResult();
        }

        [ActionName("Zone")]
        [HttpGet]
        [NoCache]
        public ActionResult Zone(Guid userId)
        {
            GuidResult guidResult = GetZoneId(userId);
            return Json(guidResult, JsonRequestBehavior.AllowGet);
        }


        [ActionName("Energy")]
        [HttpGet]
        [NoCache]
        public ActionResult Energy(Guid userId)
        {
            EnergyResult result = GetEnergyResult(userId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ActionName("SightRadius")]
        [HttpGet]
        [NoCache]
        public ActionResult SightRadius(Guid userId)
        {
            IntResult result = new IntResult { Value = _userSightRadiusProvider.GetUserSightRadius(userId, DateTime.Now) };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ActionName("AttackPower")]
        [HttpGet]
        [NoCache]
        public ActionResult AttackPower(Guid userId)
        {
            IntResult intResult = GetAttackPower(userId);
            return Json(intResult, JsonRequestBehavior.AllowGet);
        }

        [ActionName("Money")]
        [HttpGet]
        [NoCache]
        public ActionResult Money(Guid userId)
        {
            IntResult result = GetMoney(userId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ActionName("BuyItem")]
        [HttpPost]
        [NoCache]
        public ActionResult BuyItem(Guid userId, Guid itemId)
        {
            _shopDirector.BuyItem(userId, itemId);

            return new EmptyResult { };
        }

        [ActionName("Items")]
        [HttpGet]
        [NoCache]
        public ActionResult Items(Guid userId)
        {
            List<Item> items = GetItems(userId);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [ActionName("UseItem")]
        [HttpPost]
        [NoCache]
        public ActionResult UseItem(Guid userId, Guid itemId)
        {
            _itemUsageDirector.UseItem(userId, itemId);
            return new EmptyResult();
        }

        [ActionName("Achievements")]
        [HttpGet]
        [NoCache]
        public ActionResult Achievements(Guid userId)
        {
            return Json(_achievementProvider.GetAchievementsForUser(userId), JsonRequestBehavior.AllowGet);
        }

        [ActionName("Level")]
        [HttpGet]
        [NoCache]
        public ActionResult Level(Guid userId)
        {
            LevelResult levelResult = GetLevel(userId);

            return Json(levelResult, JsonRequestBehavior.AllowGet);
        }

        [ActionName("Stats")]
        [HttpGet]
        [NoCache]
        public ActionResult Stats(Guid userId)
        {
            UserStats userStats = new UserStats();
            userStats.UserNode = GetUserNode(userId);
            userStats.AttackPower = GetAttackPower(userId);
            userStats.EnergyResult = GetEnergyResult(userId);
            userStats.Items = GetItems(userId);
            userStats.LevelResult = GetLevel(userId);
            userStats.Money = GetMoney(userId);
            userStats.Zone = GetZoneId(userId);
            return Json(userStats, JsonRequestBehavior.AllowGet);
        }

        private UserNode GetUserNode(Guid? userId)
        {
            UserNode userNode;
            if (!userId.HasValue)
            {
                userNode = null;
            }
            else
            {
                IUser currentUser = _userRetriever.GetUserById(userId.Value);
                if (currentUser == null)
                {
                    userNode = null;
                }
                else
                {
                    userNode = new UserNode
                    {
                        Id = currentUser.Id,
                        ZoneId = currentUser.ZoneId,
                        Latitude = currentUser.Latitude,
                        Longitude = currentUser.Longitude,
                        Name = currentUser.DisplayName
                    };
                }
            }
            return userNode;
        }

        private IntResult GetAttackPower(Guid userId)
        {
            IntResult intResult = new IntResult { Value = _userAttackPowerProvider.GetAttackPower(userId) };
            return intResult;
        }

        private EnergyResult GetEnergyResult(Guid userId)
        {
            EnergyResult result =
                new EnergyResult
                {
                    CurrentEnergy = _userEnergyProvider.GetUserEnergy(userId, DateTime.Now),
                    TotalEnergy = _userEnergyProvider.GetUserMaxEnergy(userId)
                };

            return result;
        }

        private List<Item> GetItems(Guid userId)
        {
            List<IItem> userItems = _userItemRetriever.GetUserItems(userId);
            List<Item> items = new List<Item>();

            items = userItems.Select(c => new Item
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price,
                IsOneTimeUse = c.IsOneTimeUse
            }).ToList();
            return items;
        }

        private LevelResult GetLevel(Guid userId)
        {
            LevelResult levelResult = new LevelResult();
            levelResult.CurrentLevel = _userLevelService.GetCurrentLevelForUser(userId);
            levelResult.ZombiesKilled = _userCountsRetriever.GetZombieKillCountForUser(userId);
            levelResult.ZombiesNeededForNextLevel = _userLevelService.GetZombieCountForLevelUp(levelResult.CurrentLevel);
            return levelResult;
        }

        private IntResult GetMoney(Guid userId)
        {
            IntResult result = new IntResult { Value = _userRetriever.GetCurrentMoney(userId) };
            return result;
        }

        private GuidResult GetZoneId(Guid userId)
        {
            IUser user = _userRetriever.GetUserById(userId);
            GuidResult guidResult = new GuidResult { Value = user.ZoneId };
            return guidResult;
        }
    }
}
