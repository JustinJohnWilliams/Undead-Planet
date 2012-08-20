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
    public class UsersController : BaseController
    {
        private IZombiePackRetriever _zombiePackRetriever;
        private IHotZoneRetriever _hotZoneRetriever;
        private IUserRetriever _userRetriever;
        private IUserSaver _userSaver;
        private IUserZombiePackProgressRetriever _userHotZoneProgressRetriever;
        private IUserItemRetriever _userItemRetriever;
        private IUserStatsRetriever _userStatsRetriever;

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
            _zombiePackRetriever = DependancyInjection.Instance.Resolve<IZombiePackRetriever>();
            _hotZoneRetriever = DependancyInjection.Instance.Resolve<IHotZoneRetriever>();
            _userRetriever = DependancyInjection.Instance.Resolve<IUserRetriever>();
            _userSaver = DependancyInjection.Instance.Resolve<IUserSaver>();
            _userHotZoneProgressRetriever = DependancyInjection.Instance.Resolve<IUserZombiePackProgressRetriever>();
            _userItemRetriever = DependancyInjection.Instance.Resolve<IUserItemRetriever>();

            _distanceCalculator = DependancyInjection.Instance.Resolve<IDistanceCalculator>();
            _userEnergyProvider = DependancyInjection.Instance.Resolve<IUserEnergyProvider>();
            _userMover = DependancyInjection.Instance.Resolve<IUserMoveDirector>();
            _shopDirector = DependancyInjection.Instance.Resolve<IShopDirector>();
            _userAttackPowerProvider = DependancyInjection.Instance.Resolve<IUserAttackPowerProvider>();
            _userSightRadiusProvider = DependancyInjection.Instance.Resolve<IUserSightRadiusProvider>();
            _itemUsageDirector = DependancyInjection.Instance.Resolve<IItemUsageDirector>();
            _achievementProvider = DependancyInjection.Instance.Resolve<IAchievementProvider>();
            _userLevelService = DependancyInjection.Instance.Resolve<IUserLevelService>();
            _userStatsRetriever = DependancyInjection.Instance.Resolve<IUserStatsRetriever>();
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
            UserInGameStats userStats = new UserInGameStats();
            userStats.UserNode = GetUserNode(userId);
            userStats.AttackPower = GetAttackPower(userId);
            userStats.EnergyResult = GetEnergyResult(userId);
            userStats.Items = GetItems(userId);
            userStats.LevelResult = GetLevel(userId);
            userStats.Money = GetMoney(userId);
            userStats.Zone = GetZoneId(userId);
            userStats.ZombiesDestroyed = GetZombiesDestroyed(userId);
            userStats.MaxItems = GetMaxItems(userId);
            return Json(userStats, JsonRequestBehavior.AllowGet);
        }

        private IntResult GetMaxItems(Guid userId)
        {
            return new IntResult { Value = _userRetriever.GetCurrentBaseSlots(userId) };
        }

        private LongResult GetZombiesDestroyed(Guid userId)
        {
            return new LongResult
            {
                Value = _userStatsRetriever.GetStats(userId).ZombiesKilled
            };
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
            levelResult.ZombiesKilled = _userStatsRetriever.GetStats(userId).ZombiesKilled;
            levelResult.ZombiesNeededForNextLevel = _userLevelService.GetZombieCountForLevelUp(levelResult.CurrentLevel);
            levelResult.ZombiesKilledLastLevel = _userLevelService.GetZombieCountForLevelUp(levelResult.CurrentLevel - 1);


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
