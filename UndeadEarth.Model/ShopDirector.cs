using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class ShopDirector : IShopDirector
    {
        private IUserRetriever _userRetriever;
        private IItemRetriever _itemRetriever;
        private IStoreRetriever _storeRetriever;
        private IUserItemRetriever _userItemRetriever;
        private IUserSaver _userSaver;
        private IUserItemSaver _userItemSaver;

        public ShopDirector(IUserRetriever userRetriever,
            IItemRetriever itemRetriever,
            IStoreRetriever storeRetriever,
            IUserItemRetriever userItemRetriever,
            IUserSaver userSaver,
            IUserItemSaver userItemSaver)
        {
            _userRetriever = userRetriever;
            _itemRetriever = itemRetriever;
            _storeRetriever = storeRetriever;
            _userItemRetriever = userItemRetriever;
            _userSaver = userSaver;
            _userItemSaver = userItemSaver;
        }

        void IShopDirector.BuyItem(Guid userId, Guid itemId)
        {
            if (!_userRetriever.UserExists(userId))
            {
                return;
            }

            int maxItemCount = _userRetriever.GetCurrentBaseSlots(userId);

            IItem item = _itemRetriever.GetItemById(itemId);

            if (item == null)
            {
                return;
            }

            IUser user = _userRetriever.GetUserById(userId);

            if (user.Money < item.Price)
            {
                return;
            }

            if (!_storeRetriever.StoreExists(user.Latitude, user.Longitude))
            {
                return;
            }

            if (_userItemRetriever.GetUserItemCount(userId) >= maxItemCount)
            {
                return;
            }

            _userSaver.AddMoney(userId, (item.Price * -1));
            _userItemSaver.SaveUserItem(userId, itemId);

        }
    }
}
