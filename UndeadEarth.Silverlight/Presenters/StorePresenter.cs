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
using System.Collections.Generic;
using UndeadEarth.Model.Proxy;
using RestfulSilverlight.Core;
using System.Linq;

namespace UndeadEarth.Silverlight.Presenters
{
    public class StorePresenter
    {
        public interface IView
        {
            /// <summary>
            /// Alerts the view that the user can shop.
            /// </summary>
            void NotifyCanShop();

            /// <summary>
            /// Alerts the view that the user can not shop.
            /// </summary>
            void NotifyCannotShop();

            /// <summary>
            /// Populates the store's inventory
            /// </summary>
            /// <param name="items"></param>
            void PopulateStoreInventory(List<Item> items);

            /// <summary>
            /// Populates the user's money
            /// </summary>
            /// <param name="money"></param>
            void PopulateUserMoney(int money);

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
        /// Reference to all the visible stores
        /// </summary>
        private List<StoreNode> _visibleStores;

        public StorePresenter(MapPresenter.IView mapPresenterView, IView view, GameContext gameContext, INotificationView notificationView)
        {
            _mapPresenterView = mapPresenterView;
            _view = view;
            _gameContext = gameContext;
            _notificationView = notificationView;
            _visibleStores = new List<StoreNode>();
            _gameContext.UserMoved += new EventHandler(_gameContext_UserMoved);
            _gameContext.HotZoneIdChanged += new EventHandler(_gameContext_HotZoneIdChanged);
            _gameContext.UserInventoryChanged += new EventHandler<UserInventoryChangedEventArgs>(_gameContext_UserInventoryChanged);
        }

        void _gameContext_HotZoneIdChanged(object sender, EventArgs e)
        {
            GetStoresForHotZone(_gameContext.HotZoneId);
        }

        private void GetStoresForHotZone(Guid hotZoneId)
        {
            AsyncDelegation ad = new AsyncDelegation();
            ad.Get<List<StoreNode>>().ForRoute(() => new { controller = "Stores", action = "InHotZone", userId = _gameContext.UserId })
              .WhenFinished(r =>
              {
                  _mapPresenterView.RemoveStores(_visibleStores);

                  _visibleStores.Clear();
                  foreach (StoreNode safeHouse in r)
                  {
                      _visibleStores.Add(safeHouse);
                  }

                  _mapPresenterView.AddStores(_visibleStores);

                  CheckCanShop();
              });

            ad.Go();
        }

        void _gameContext_UserInventoryChanged(object sender, UserInventoryChangedEventArgs e)
        {
            if (IsOnStore())
            {
                _view.PopulateUserInventory(e.Items.OrderBy(i => i.Name).ToList());
            }
        }

        void _gameContext_UserMoved(object sender, EventArgs e)
        {
            CheckCanShop();
        }

        private void CheckCanShop()
        {
            if (IsOnStore())
            {
                _view.NotifyCanShop();

                GetInventory();
                GetUserMoney();
                GetUserInventory();
            }
            else
            {
                _view.NotifyCannotShop();
            }
        }

        private bool IsOnStore()
        {
            if (_visibleStores != null)
            {
                return _visibleStores.Any(c => Math.Round(c.Latitude, 4) == Math.Round(_gameContext.UserNode.Latitude, 4) && Math.Round(c.Longitude, 4) == Math.Round(_gameContext.UserNode.Longitude, 4));
            }

            return false;
        }

        private void GetInventory()
        {
            StoreNode currentStore = _visibleStores.SingleOrDefault(c => c.Latitude == _gameContext.UserNode.Latitude && c.Longitude == _gameContext.UserNode.Longitude);
            if (currentStore == null)
            {
                return;
            }

            AsyncDelegation ad = new AsyncDelegation();
            ad.Get<List<Item>>(new
            {
                controller = "Stores",
                action = "Items",
                storeId = currentStore.Id
            })
            .WhenFinished(
            (items) =>
            {
                _view.PopulateStoreInventory(items.OrderBy(i => i.Name).ToList());
            });

            ad.Go();
        }

        private void GetUserMoney()
        {
            _view.PopulateUserMoney(_gameContext.Money);
        }

        public void Load()
        {
            _view.NotifyCannotShop();
        }

        public void Buy(Item item)
        {
            if (item.Price > _gameContext.Money)
            {
                _notificationView.Notify("You don't have enough money to buy this item.  Go hunt for more zombies and come back when you do.");
                return;
            }

            if (_gameContext.UserInventory.Count >= _gameContext.UserMaxItems)
            {
                _notificationView.Notify("You can't carry any more items.  Go to the safe house and drop some items off, then come back.");
                return;
            }

            AsyncDelegation ad = new AsyncDelegation();

            ad.Post(
                new
                {
                    controller = "Users",
                    action = "BuyItem",
                    userId = _gameContext.UserId,
                    itemId = item.Id
                })
                .WhenFinished(() => { })
                .ThenGet<IntResult>(new { controller = "Users", action = "Money", userId = _gameContext.UserId })
                .WhenFinished(r =>
                {
                    _gameContext.SetMoney(r.Value);
                    GetUserMoney();
                    GetUserInventory();
                });

            ad.Go();

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
                    _gameContext.SetUserInventory(items.OrderBy(i => i.Name).ToList());
                });

            ad.Go();
        }
    }
}
