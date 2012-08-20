using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class SafeHouseDirector : ISafeHouseDirector
    {
        private IUserRetriever _userRetriever;
        private IItemRetriever _itemRetriever;
        private ISafeHouseRetriever _safeHouseRetriever;
        private IUserItemRetriever _userItemRetriever;
        private ISafeHouseItemSaver _safeHouseItemSaver;
        private IUserItemSaver _userItemSaver;

        public SafeHouseDirector(IUserRetriever userRetriever
                               , IItemRetriever itemRetriever
                               , ISafeHouseRetriever safeHouseRetriever
                               , IUserItemRetriever userItemRetriever
                               , ISafeHouseItemSaver safeHouseItemSaver
                               , IUserItemSaver userItemSaver)
        {
            _userRetriever = userRetriever;
            _itemRetriever = itemRetriever;
            _safeHouseRetriever = safeHouseRetriever;
            _userItemRetriever = userItemRetriever;
            _safeHouseItemSaver = safeHouseItemSaver;
            _userItemSaver = userItemSaver;
        }

        void ISafeHouseDirector.TransferItemFromUserToSafeHouse(Guid userId, Guid safeHouseId, Guid itemId)
        {
            if (_userRetriever.UserExists(userId) == false)
            {
                return;
            }

            if (_safeHouseRetriever.SafeHouseExists(safeHouseId) == false)
            {
                return;
            }

            if (_itemRetriever.ItemExists(itemId) == false)
            {
                return;
            }

            if (_userItemRetriever.UserHasItem(userId, itemId) == false)
            {
                return;
            }

            Guid userItemId = _userItemRetriever.GetUserItemId(userId, itemId);

            _safeHouseItemSaver.SaveItemInSafeHouse(safeHouseId, itemId, userId);
            _userItemSaver.RemoveUserItem(userItemId);
        }

        void ISafeHouseDirector.TransferItemFromSafeHouseToUser(Guid userId, Guid safeHouseId, Guid itemId)
        {
            if (_userRetriever.UserExists(userId) == false)
            {
                return;
            }

            int maxItemCount = _userRetriever.GetCurrentBaseSlots(userId);

            if (_safeHouseRetriever.SafeHouseExists(safeHouseId) == false)
            {
                return;
            }

            if (_itemRetriever.ItemExists(itemId) == false)
            {
                return;
            }

            if (_userItemRetriever.GetUserItemCount(userId) >= maxItemCount)
            {
                return;
            }

            if (_safeHouseRetriever.SafeHouseHasItem(itemId, userId) == false)
            {
                return;
            }

            Guid safeHouseUserItemId = _safeHouseRetriever.GetSafeHouseItemId(itemId, userId);

            _userItemSaver.SaveUserItem(userId, itemId);
            _safeHouseItemSaver.RemoveSafeHouseItem(safeHouseUserItemId);
        }
    }
}
