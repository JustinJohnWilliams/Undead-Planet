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
using System.Collections.Generic;
using UndeadEarth.Silverlight.Model;
using RestfulSilverlight.Core;
using System.Linq;

namespace UndeadEarth.Silverlight.Presenters
{
    public class SafeHousePresenter
    {
        public interface IView
        {
            /// <summary>
            /// Alerts the view the user can visit the safe house
            /// </summary>
            void NotifyCanVisitSafeHouse();

            /// <summary>
            /// Alerts the view the user can not visit the safe house
            /// </summary>
            void NotifyCannotVisitSafeHouse();

            /// <summary>
            /// Populates the safe houses inventory
            /// </summary>
            /// <param name="items"></param>
            void PopulateSafeHouseInventory(List<Item> items);

            /// <summary>
            /// Populates the user's items
            /// </summary>
            /// <param name="items"></param>
            void PopulateUserInventory(List<Item> items);
        }

        private MapPresenter.IView _mapPresenterView;
        private GameContext _gameContext;
        private IView _view;
        private INotificationView _notificationView;

        /// <summary>
        /// Reference to all visible safe houses
        /// </summary>
        private List<SafeHouseNode> _visibleSafeHouses;

        public SafeHousePresenter(MapPresenter.IView mapPresenterView, IView view, GameContext gameContext, INotificationView notificationView)
        {
            _mapPresenterView = mapPresenterView;
            _view = view;
            _gameContext = gameContext;
            _notificationView = notificationView;
            _visibleSafeHouses = new List<SafeHouseNode>();
            _gameContext.UserMoved += new EventHandler(_gameContext_UserMoved);
            _gameContext.UserInventoryChanged += new EventHandler<UserInventoryChangedEventArgs>(_gameContext_UserInventoryChanged);
            _gameContext.HotZoneIdChanged += new EventHandler(_gameContext_HotZoneIdChanged);
        }

        void _gameContext_HotZoneIdChanged(object sender, EventArgs e)
        {
            GetSafeHousesForHotZone(_gameContext.HotZoneId);
        }

        private void GetSafeHousesForHotZone(Guid hotZoneId)
        {
            AsyncDelegation ad = new AsyncDelegation();
            ad.Get<List<SafeHouseNode>>().ForRoute(() => new { controller = "SafeHouses", action = "InHotZone", userId = _gameContext.UserId })
              .WhenFinished(r =>
              {
                  _mapPresenterView.RemoveSafeHouses(_visibleSafeHouses);

                  _visibleSafeHouses.Clear();
                  foreach (SafeHouseNode safeHouse in r)
                  {
                      _visibleSafeHouses.Add(safeHouse);
                  }

                  _mapPresenterView.AddSafeHouses(_visibleSafeHouses);

                  CheckCanVisitSafeHouse();
              });

            ad.Go();
        }

        void _gameContext_UserInventoryChanged(object sender, UserInventoryChangedEventArgs e)
        {
            _view.PopulateUserInventory(e.Items.OrderBy(s => s.Name).ToList());
        }

        void _gameContext_UserMoved(object sender, EventArgs e)
        {
            CheckCanVisitSafeHouse();
        }

        private void CheckCanVisitSafeHouse()
        {
            if (IsAtSafeHouse())
            {
                _view.NotifyCanVisitSafeHouse();

                GetSafeHouseInventory();
                GetUserInventory();
            }
            else
            {
                _view.NotifyCannotVisitSafeHouse();
            }

        }

        private bool IsAtSafeHouse()
        {
            if (_visibleSafeHouses != null)
            {
                return _visibleSafeHouses.Any(c => Math.Round(c.Latitude, 4) == Math.Round(_gameContext.UserNode.Latitude, 4)
                                                && Math.Round(c.Longitude, 4) == Math.Round(_gameContext.UserNode.Longitude, 4));
            }

            return false;
        }

        private void GetUserInventory()
        {
            AsyncDelegation ad = new AsyncDelegation();

            ad.Get<List<Item>>(
                new
                {
                    controller = "Users",
                    action = "Items",
                    userId = _gameContext.UserId
                })
                .WhenFinished(
                (items) =>
                {
                    var orderedList = items.OrderBy(s => s.Name).ToList();
                    _view.PopulateUserInventory(orderedList);
                    _gameContext.SetUserInventory(orderedList);
                });

            ad.Go();
        }

        private void GetSafeHouseInventory()
        {
            SafeHouseNode currentSafeHouse = _visibleSafeHouses.SingleOrDefault(c => c.Latitude == _gameContext.UserNode.Latitude && c.Longitude == _gameContext.UserNode.Longitude);
            if (currentSafeHouse == null)
            {
                return;
            }

            AsyncDelegation ad = new AsyncDelegation();

            ad.Get<List<Item>>(
                new
                {
                    controller = "SafeHouses",
                    action = "Items",
                    safeHouseId = currentSafeHouse.Id,
                    userId = _gameContext.UserId
                })
                .WhenFinished(
                (items) =>
                {
                    _view.PopulateSafeHouseInventory(items.OrderBy(i => i.Name).ToList());
                });

            ad.Go();
        }

        internal void Load()
        {
            _view.NotifyCannotVisitSafeHouse();
        }

        internal void TransferItemFromUserToSafeHouse(Guid itemId)
        {
            SafeHouseNode currentSafeHouse = GetCurrentSafeHouse();
            if (currentSafeHouse == null)
            {
                return;
            }

            AsyncDelegation ad = new AsyncDelegation();

            ad.Post(
                new
                {
                    controller = "SafeHouses",
                    action = "StoreItem",
                    userId = _gameContext.UserId,
                    safeHouseId = currentSafeHouse.Id,
                    itemId = itemId
                })
                .WhenFinished(() =>
                {
                    GetUserInventory();
                    GetSafeHouseInventory();
                });

            ad.Go();
        }

        internal void TransferItemFromSafeHouseToUser(Guid itemId)
        {
            if (_gameContext.UserInventory.Count >= _gameContext.UserMaxItems)
            {
                _notificationView.Notify("You cannot equip any more items.  Move items to the safe house first to create more space.");
                return;
            }

            SafeHouseNode currentSafeHouse = GetCurrentSafeHouse();
            if (currentSafeHouse == null)
            {
                return;
            }

            AsyncDelegation ad = new AsyncDelegation();

            ad.Post(
                new
                {
                    controller = "SafeHouses",
                    action = "UserRetrieveItem",
                    userId = _gameContext.UserId,
                    safeHouseId = currentSafeHouse.Id,
                    itemId = itemId
                })
                .WhenFinished(() =>
                {
                    GetUserInventory();
                    GetSafeHouseInventory();
                });

            ad.Go();
        }

        private SafeHouseNode GetCurrentSafeHouse()
        {
            return _visibleSafeHouses.SingleOrDefault(c => c.Latitude == _gameContext.UserNode.Latitude && c.Longitude == _gameContext.UserNode.Longitude);
        }
    }
}
