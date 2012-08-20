using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UndeadEarth.Model.Proxy;
using UndeadEarth.Silverlight.Proxy;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using RestfulSilverlight.Core;
using Microsoft.Maps.MapControl;
using UndeadEarth.Silverlight.Model;
using UndeadEarth.Model;
using UndeadEarth.Contract;

namespace UndeadEarth.Silverlight.Presenters
{
    public class UserPresenter
    {
        private const int energyRegenAmount = 10;
        public interface IView
        {
            /// <summary>
            /// Tells the view to populate the usersEnergy.
            /// </summary>
            void PopulateEnergy(int currentEnergy, int totalEnergy);

            /// <summary>
            /// Delegates to the view to populate the money a User has.
            /// </summary>
            /// <param name="money"></param>
            void PopulateMoney(int money);

            /// <summary>
            /// Delegates to the view to populate user inventory.
            /// </summary>
            /// <param name="items"></param>
            void PopulateInventory(List<KeyValuePair<Item, string>> items);

            /// <summary>
            /// Delegates to the view to populate attack power.
            /// </summary>
            /// <param name="attackPower"></param>
            void PopulateAttackPower(int attackPower);

            /// <summary>
            /// Delegates to the view to populate achivements.
            /// </summary>
            /// <param name="achievements"></param>
            void PopulateAchievements(List<Achievement> achievements);

            /// <summary>
            /// Notification to the view that users has achievements.
            /// </summary>
            /// <param name="achievements"></param>
            void NotifyNewAchievement(List<string> achievements);

            /// <summary>
            /// Notifies the view of user's current level and populates progress bar
            /// to show how many more kills user must get to reach next level.
            /// </summary>
            /// <param name="currentLevel"></param>
            /// <param name="currentKills"></param>
            /// <param name="neededKills"></param>
            /// <param name="lastLevelKills"></param>
            void PopulateUserLevel(int currentLevel, long currentKills, long neededKills, long lastLevelKills);

            /// <summary>
            /// Delegates to the view to show the tutorial page.
            /// </summary>
            void ShowTutorial();

            /// <summary>
            /// Notification to the view that the user has leveled up.
            /// </summary>
            void NotifyLevelUp();

            /// <summary>
            /// Notification to the view that the user will recieve a certain amount of energy in a given time period.
            /// </summary>
            /// <param name="energyAmount"></param>
            /// <param name="secondsUntil"></param>
            void NotifyMoreEnergyCountDown(int energyAmount, int secondsUntil);
        }

        /// <summary>
        /// View to interact with.
        /// </summary>
        private IView _view;

        /// <summary>
        /// Delay execution for energy.
        /// </summary>
        private DelayExecution _energyExecution;

        private DelayExecution _energyIn45;
        private DelayExecution _energyIn30;
        private DelayExecution _energyIn15;

        /// <summary>
        /// Represents the game context that will be shared accross all presenters.
        /// </summary>
        private GameContext _gameContext;

        /// <summary>
        /// Represents the map.
        /// </summary>
        private MapPresenter.IView _mapPresenterView;

        /// <summary>
        /// Represents the notification view for the game.
        /// </summary>
        private INotificationView _notificationView;

        /// <summary>
        /// List of all achievements.
        /// </summary>
        private List<string> _lastAchievements;

        /// <summary>
        /// Last level.
        /// </summary>
        private int _lastLevel;

        /// <summary>
        /// Constructor.
        /// </summary>
        public UserPresenter(MapPresenter.IView mapPresenterView, IView view, GameContext gameContext, INotificationView notificationView)
        {
            _gameContext = gameContext;
            _view = view;
            _mapPresenterView = mapPresenterView;
            _notificationView = notificationView;
            _energyExecution = new DelayExecution();
            _energyIn45 = new DelayExecution();
            _energyIn30 = new DelayExecution();
            _energyIn15 = new DelayExecution();
            _gameContext.UserEnergyChanged += new EventHandler<UserEnergyChangedEventArgs>(_gameContext_UserEnergyChanged);
            _gameContext.UserMoneyChanged += new EventHandler<UserMoneyChangedEventArgs>(_gameContext_UserMoneyChanged);
            _gameContext.UserInventoryChanged += new EventHandler<UserInventoryChangedEventArgs>(_gameContext_UserInventoryChanged);
            _gameContext.UserAttackPowerChanged += new EventHandler(_gameContext_UserPowerChanged);
            _gameContext.UserInitalized += new EventHandler(_gameContext_UserInitalized);
            _gameContext.UserLevelChanged += new EventHandler<UserLevelChangedEventArgs>(_gameContext_UserLevelChanged);
            AsyncDelegation.BaseUri = _gameContext.BaseUri;
        }

        void _gameContext_UserInitalized(object sender, EventArgs e)
        {
            GetAchievements();
        }

        private void GetAchievements()
        {
            AsyncDelegation ad = new AsyncDelegation();

            ad.Get<List<string>>(new { controller = "Users", action = "Achievements", userId = _gameContext.UserId })
              .WhenFinished(r =>
                  {
                      string uri = String.Concat(AsyncDelegation.BaseUri, "Content/", "images/", "medal.png");
                      List<Achievement> achievements = new List<Achievement>();
                      foreach (string achievement in r)
                      {
                          achievements.Add(new Achievement { Description = achievement, ImagePath = uri });
                      }

                      _view.PopulateAchievements(achievements);

                      if (_lastAchievements != null)
                      {
                          List<string> newAchievements = r.Where(s => _lastAchievements.Contains(s) == false).Select(s => "Achievement Awarded: " + s).ToList();
                          if (newAchievements.Count() > 0)
                          {
                              _view.NotifyNewAchievement(newAchievements);
                          }
                      }

                      _lastAchievements = r;
                  });

            ad.Go();
        }

        void _gameContext_UserPowerChanged(object sender, EventArgs e)
        {
            _view.PopulateAttackPower(_gameContext.UserAttackPower);
        }

        void _gameContext_UserInventoryChanged(object sender, UserInventoryChangedEventArgs e)
        {
            var counts = from item in e.Items
                         group item by item.Id into resultGroup
                         select new
                         {
                             ItemId = resultGroup.Key,
                             Count = resultGroup.Count()
                         };


            List<KeyValuePair<Item, string>> finalList = new List<KeyValuePair<Item, string>>();

            foreach (var count in counts)
            {
                finalList.Add(new KeyValuePair<Item, string>(e.Items.First(s => s.Id == count.ItemId), "x " + count.Count.ToString()));
            }

            if (e.Items.Count == 0)
            {
                Item emptyItem = new Item { Id = Guid.Empty, Description = "You have no items.  Go to the store to buy some.", IsOneTimeUse = false, Name = "(None)", Price = 0 };

                finalList.Add(new KeyValuePair<Item, string>(emptyItem, ""));
            }

            _view.PopulateInventory(finalList);

            //if the inventory has changed, re calc the attack power of user and the energy of the user
            RefreshAttackPowerAndEnergy();
        }

        void _gameContext_UserMoneyChanged(object sender, UserMoneyChangedEventArgs e)
        {
            _view.PopulateMoney(e.Money);
        }

        void _gameContext_UserEnergyChanged(object sender, UserEnergyChangedEventArgs e)
        {
            SetUserEnergy(_gameContext.UserEnergy);
            GetAchievements(); //if the energy has changed, then the user has either moved or hunted, get the achievments
        }

        void _gameContext_UserLevelChanged(object sender, UserLevelChangedEventArgs e)
        {
            if (_lastLevel != e.Level.CurrentLevel && e.Level.CurrentLevel != 1)
            {
                _lastLevel = e.Level.CurrentLevel;
                _view.NotifyLevelUp();
            }

            SetUserLevel(_gameContext.UserLevel);
        }


        private int _defaultZoomLevel = 8;

        /// <summary>
        /// Loads the game.
        /// </summary>
        public void Load()
        {
            AsyncDelegation ad = new AsyncDelegation();

            ad.Get<UserInGameStats>(new { controller = "Users", action = "Stats", userId = _gameContext.UserId })
              .WhenFinished(
              (data) =>
              {
                  _gameContext.UserNode = data.UserNode;

                  _lastLevel = data.LevelResult.CurrentLevel;
                  _mapPresenterView.InitializeUser(data.UserNode);
                  _mapPresenterView.MoveToUser(_defaultZoomLevel);
                  _gameContext.SetUserEnergy(data.EnergyResult);
                  _gameContext.SetUserInventory(data.Items);
                  _gameContext.SetUserAttackPower(data.AttackPower.Value);
                  _gameContext.SetHotZoneId(data.Zone.Value);
                  _gameContext.SetMoney(data.Money.Value);
                  _gameContext.SetUserMaxItems(data.MaxItems.Value);
                  _gameContext.SetUserLevel(data.LevelResult);
                  _gameContext.OnUserInitialized();

                  if (data.ZombiesDestroyed.Value == 0)
                  {
                      _view.ShowTutorial();
                  }
              });

            ad.Go();

            PollForEnergy();
        }

        /// <summary>
        /// Polls at regular intervals for energy.
        /// </summary>
        private void PollForEnergy()
        {
            AsyncDelegation ad = new AsyncDelegation();
            ad.Get<EnergyResult>(new { controller = "Users", action = "Energy", userId = _gameContext.UserId })
              .WhenFinished(r => _gameContext.SetUserEnergy(r));

            ad.Go();

            //start the energy and sight radius polling
            _energyExecution.SetTimeout(60 * 1000, () => PollForEnergy());
            _view.NotifyMoreEnergyCountDown(energyRegenAmount, 60);
            _energyIn45.SetTimeout(15 * 1000, () => _view.NotifyMoreEnergyCountDown(energyRegenAmount, 45));
            _energyIn30.SetTimeout(30 * 1000, () => _view.NotifyMoreEnergyCountDown(energyRegenAmount, 30));
            _energyIn15.SetTimeout(45 * 1000, () => _view.NotifyMoreEnergyCountDown(energyRegenAmount, 15));
        }

        private void SetUserEnergy(EnergyResult energyResult)
        {
            _view.PopulateEnergy(energyResult.CurrentEnergy, energyResult.TotalEnergy);
            if (_gameContext.UserNode != null)
            {
                var locations = GetCircle(new Location { Latitude = _gameContext.UserNode.Latitude, Longitude = _gameContext.UserNode.Longitude }, energyResult.CurrentEnergy);
                _mapPresenterView.UpdateUserMoveRadius(locations);
            }
        }

        private void SetUserLevel(LevelResult levelResult)
        {
            _view.PopulateUserLevel(levelResult.CurrentLevel, levelResult.ZombiesKilled, levelResult.ZombiesNeededForNextLevel, levelResult.ZombiesKilledLastLevel);
        }

        /// <summary>
        /// Request from the view to update user's location.
        /// </summary>
        public void Move(double latitude, double longitude)
        {
            //dont bother moving if the latitude longitude past in is the current user's latitude and longitude
            double currentLatitude = _gameContext.UserNode.Latitude;
            double currentLongitude = _gameContext.UserNode.Longitude;

            if (latitude == currentLatitude && longitude == currentLongitude) return;

            AsyncDelegation ad = new AsyncDelegation();

            ad.Post(new { controller = "Users", action = "Location", userId = _gameContext.UserId, latitude = latitude, longitude = longitude })
              .WhenFinished(() => { })
              .ThenGet<GuidResult>(new { controller = "Users", action = "Zone", userId = _gameContext.UserId })
              .WhenFinished(r => _gameContext.SetHotZoneId(r.Value))
              .ThenGet<UserNode>(new { controller = "Users", action = "Index", userId = _gameContext.UserId })
              .WhenFinished(
              (userNode) =>
              {
                  //return if user is in the same location.  nothing needs to be updated.
                  if (currentLatitude == userNode.Latitude && currentLongitude == userNode.Longitude)
                  {
                      _notificationView.Notify("You do not have enough energy to move there (the green circle is your maximum move radius).  You can equip gear to increase you maximum energy, you can use an item to replenish your energy (such as a tent), or you can wait.  Over time your energy will fill up to the maximum.");
                      return;
                  }

                  _gameContext.UserNode = userNode;
                  _mapPresenterView.UpdateUserLocation(_gameContext.UserNode.Latitude, _gameContext.UserNode.Longitude);
                  _gameContext.NotifyMoveComplete();

                  _mapPresenterView.MoveToUser(_mapPresenterView.GetZoomLevel());
              })
              .ThenGet<EnergyResult>().ForRoute(() => new
              {
                  controller = "Users",
                  action = "Energy",
                  userId = _gameContext.UserId
              })
              .WhenFinished(r => _gameContext.SetUserEnergy(r));

            ad.Go();
        }

        private List<Location> GetCircle(Location center, double radius)
        {
            double earthRadius = 3958.75587;
            double latitude = center.Latitude * Math.PI / 180;
            double longitude = center.Longitude * Math.PI / 180;
            double distanceRatio = radius / earthRadius;
            List<Location> locations = new List<Location>();
            for (int degree = 0; degree <= 360; degree++)
            {
                Location point = GetLocation(latitude, longitude, distanceRatio, degree);

                locations.Add(point);
            }

            return locations;
        }

        private Location GetLocation(double latitude, double longitude, double distanceRatio, int degree)
        {
            Location point = new Location();

            double radian = degree * Math.PI / 180;

            point.Latitude = Math.Asin(Math.Sin(latitude) * Math.Cos(distanceRatio) + Math.Cos(latitude) * Math.Sin(distanceRatio * Math.Cos(radian)));
            point.Longitude = ((longitude + Math.Atan2(Math.Sin(radian) * Math.Sin(distanceRatio) * Math.Cos(latitude), Math.Cos(distanceRatio) - Math.Sin(latitude) * Math.Sin(point.Latitude))) * 180) / Math.PI;
            point.Latitude = (point.Latitude * 180) / Math.PI;

            return point;
        }

        private void RefreshAttackPowerAndEnergy()
        {
            AsyncDelegation ad = new AsyncDelegation();

            ad.Get<IntResult>(new { controller = "Users", action = "AttackPower", userId = _gameContext.UserId })
              .WhenFinished(r => _gameContext.SetUserAttackPower(r.Value))
              .ThenGet<EnergyResult>(new { controller = "Users", action = "Energy", userId = _gameContext.UserId })
              .WhenFinished(r => _gameContext.SetUserEnergy(r));

            ad.Go();
        }

        /// <summary>
        /// Request from the view for a user to use an item.
        /// </summary>
        /// <param name="item"></param>
        public void UseItem(Item item)
        {
            AsyncDelegation ad = new AsyncDelegation();

            ad.Post(new { controller = "Users", action = "UseItem", userId = _gameContext.UserId, itemId = item.Id })
              .WhenFinished(() => { })
              .ThenGet<List<Item>>(new { controller = "Users", action = "Items", userId = _gameContext.UserId })
              .WhenFinished(r => _gameContext.SetUserInventory(r.OrderBy(i => i.Name).ToList()));

            ad.Go();
        }

        public void RequestGoToUser()
        {
            _mapPresenterView.MoveToUser(_defaultZoomLevel);
        }

        public int GetDistanceFromUser(double latitude, double longitude)
        {
            IDistanceCalculator distanceCalculator = new DistanceCalculator();
            return Convert.ToInt32(distanceCalculator.CalculateMiles(_gameContext.UserNode.Latitude, _gameContext.UserNode.Longitude, latitude, longitude));
        }
    }
}
