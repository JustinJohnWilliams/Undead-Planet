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
using UndeadEarth.Silverlight.Model;
using System.Linq;
using RestfulSilverlight.Core;
using UndeadEarth.Model.Proxy;
using System.Collections.Generic;

namespace UndeadEarth.Silverlight.Presenters
{
    public class HuntPresenter
    {
        public interface IView
        {
            /// <summary>
            /// Tells the view that the user can hunt.
            /// </summary>
            void NotifyCanHunt();

            /// <summary>
            /// Tells the view that the user cannot hunt.
            /// </summary>
            void NotifyCannotHunt();

            /// <summary>
            /// Delegate to the view to show the destruction percentage.
            /// </summary>
            /// <param name="percentage"></param>
            void PopulateDestructionProgress(int zombiesLeft, int maxZombies, int costForHunt);

            /// <summary>
            /// Delegates to the view to populate attack power.
            /// </summary>
            /// <param name="attackPower"></param>
            void PopulateUserAttackPower(int attackPower);
        }

        private IView _view;
        private MapPresenter.IView _mapPresenterView;
        private GameContext _gameContext;

        /// <summary>
        /// Represents all visible zombie packs.
        /// </summary>
        private List<Node> _visibleZombiePacks;

        /// <summary>
        /// Represents the current zombie pack id for user.
        /// </summary>
        private Guid _currentZombiePackId;

        /// <summary>
        /// Represents the notification view.
        /// </summary>
        private INotificationView _notificationView;

        private Guid _currentHotZoneId;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="gameContext"></param>
        public HuntPresenter(MapPresenter.IView mapPresenterView, IView view, GameContext gameContext, INotificationView notificationView)
        {
            //set the view and the game context
            _view = view;
            _mapPresenterView = mapPresenterView;
            _gameContext = gameContext;
            _currentZombiePackId = Guid.Empty;
            _notificationView = notificationView;
            _visibleZombiePacks = new List<Node>();


            //wire up to the user moved event for the game context
            _gameContext.UserAttackPowerChanged += new EventHandler(_gameContext_UserAttackPowerChanged);
            _gameContext.HotZoneIdChanged += new EventHandler(_gameContext_HotZoneIdChanged);
            _gameContext.UserMoved += new EventHandler(_gameContext_UserMoved);
        }

        void _gameContext_UserMoved(object sender, EventArgs e)
        {
            UpdateHuntStatus();
        }

        void _gameContext_HotZoneIdChanged(object sender, EventArgs e)
        {
            GetZombiePacksForHotZone(_gameContext.HotZoneId);
        }

        void _gameContext_UserAttackPowerChanged(object sender, EventArgs e)
        {
            if (IsUserOnZombiePack())
            {
                _view.PopulateUserAttackPower(_gameContext.UserAttackPower);
            }
        }

        private void GetZombiePacksForHotZone(Guid hotZoneId)
        {
            AsyncDelegation ad = new AsyncDelegation();
            ad.Get<List<ZombiePackNode>>().ForRoute(() => new { controller = "ZombiePacks", action = "InHotzone", userId = _gameContext.UserId })
              .WhenFinished(r =>
               {
                   _mapPresenterView.RemoveZombiePacks(_visibleZombiePacks.Select(s => s.Id).ToList());

                   _visibleZombiePacks.Clear();
                   foreach (ZombiePackNode zombiePack in r)
                   {
                       _visibleZombiePacks.Add(zombiePack);
                   }

                   _mapPresenterView.AddZombiePacks(_visibleZombiePacks);

                   UpdateHuntStatus();
               });

            ad.Go();
        }

        private void UpdateZombiePacksIfApplicable()
        {
            AsyncDelegation ad = new AsyncDelegation();
            ad.Get<GuidResult>(new { controller = "Users", action = "Zone", userId = _gameContext.UserId })
              .WhenFinished(r =>
                  {
                      if (r.Value == _currentHotZoneId)
                      {
                          UpdateHuntStatus();
                      }
                      else
                      {
                          _currentHotZoneId = r.Value;
                          GetZombiePacksForHotZone(_currentHotZoneId);
                      }
                  });

            ad.Go();
        }

        private void UpdateHuntStatus()
        {
            bool isUserOnZombiePack = IsUserOnZombiePack();
            if (isUserOnZombiePack)
            {
                //if the user can hunt get the info for the zombie pack
                _currentZombiePackId = _visibleZombiePacks.First(c => c.Latitude == _gameContext.UserNode.Latitude && c.Longitude == _gameContext.UserNode.Longitude).Id;
                _view.NotifyCanHunt();
                _view.PopulateUserAttackPower(_gameContext.UserAttackPower);
                GetZombiePackInfo();
            }
            else
            {
                _currentZombiePackId = Guid.Empty;
                _view.NotifyCannotHunt();
            }
        }

        private bool IsUserOnZombiePack()
        {
            return _visibleZombiePacks.Any(c => c.Latitude == _gameContext.UserNode.Latitude && c.Longitude == _gameContext.UserNode.Longitude);
        }

        private ZombiePackProgress _currentZombiePackInfo;
        private void GetZombiePackInfo()
        {
            if (_currentZombiePackId == Guid.Empty) return;

            AsyncDelegation ad = new AsyncDelegation();
            ad.Get<ZombiePackProgress>(new
            {
                controller = "ZombiePacks",
                action = "DestructionProgress",
                userId = _gameContext.UserId,
                zombiePackId = _currentZombiePackId
            })
            .WhenFinished(r =>
            {
                _currentZombiePackInfo = r;
                _view.PopulateDestructionProgress(r.ZombiesLeft, r.MaxZombies, r.CostPerHunt);
            });

            ad.Go();
        }

        /// <summary>
        /// Request from the view to hunt.
        /// </summary>
        public void Hunt()
        {
            //return if the current zombie pack id is 0
            if (_currentZombiePackId == Guid.Empty)
            {
                return;
            }

            if (_currentZombiePackInfo != null && _currentZombiePackInfo.CostPerHunt > _gameContext.UserEnergy.CurrentEnergy)
            {
                _notificationView.Notify("You do not have enough energy to hunt here.  Use an item to replenish your energy, equip armor to raise your maximum energy, or simply wait for your energy to replenish.  You will regain energy over time.");
                return;
            }

            AsyncDelegation ad = new AsyncDelegation();

            //perform hunt
            ad.Post(
              new
              {
                  controller = "ZombiePacks",
                  action = "Hunt",
                  userId = _gameContext.UserId,
                  zombiePackId = _currentZombiePackId
              })
                //when finished do nothing
              .WhenFinished(() => { })
              .ThenGet<UserInGameStats>(new { controller = "Users", action = "Stats", userId = _gameContext.UserId })
              .WhenFinished(
              (data) =>
              {
                  _gameContext.SetUserEnergy(data.EnergyResult);
                  _gameContext.SetUserInventory(data.Items);
                  _gameContext.SetUserAttackPower(data.AttackPower.Value);
                  _gameContext.SetMoney(data.Money.Value);
                  _gameContext.SetUserMaxItems(data.MaxItems.Value);
                  _gameContext.SetUserLevel(data.LevelResult);
              })
              //check to see if the zombie pack is cleared
              .ThenGet<BooleanResult>(
              new
              {
                  controller = "ZombiePacks",
                  action = "IsCleared",
                  userId = _gameContext.UserId,
                  zombiePackId = _currentZombiePackId
              })
                //when the zombie pack data is returned
              .WhenFinished(
              (booleanResult) =>
              {
                  //if the zombie pack has been destroyed
                  if (booleanResult.Value == true)
                  {
                      //find the zombie pack
                      Node node = _visibleZombiePacks.FirstOrDefault(c => c.Id == _currentZombiePackId);
                      if (node != null)
                      {
                          //remove the node from visible nodes
                          _visibleZombiePacks.Remove(node);

                          //notify to the game context that a zombie pack has been destroyed
                          List<Guid> nodes = new List<Guid>();
                          nodes.Add(node.Id);
                          _mapPresenterView.RemoveZombiePacks(nodes);

                          //set the zombie pack to guid.empty
                          _currentZombiePackId = Guid.Empty;

                          //if the zombie pack has been cleared, determine if the hotzone has been cleared and update the hunt status of the user
                          _gameContext.NotifyZombiePackDestroyed(node.Id);
                          UpdateHuntStatus();
                      }
                  }
                  else
                  {
                      //if the zombie pack hasn't been destroyed, update hunt status to reflect new destruction amounts.
                      UpdateHuntStatus();
                  }
              });

            ad.Go();
        }

        public void Load()
        {
            _view.NotifyCannotHunt();
        }
    }
}
