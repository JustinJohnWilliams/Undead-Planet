using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class ItemUsageDirector : IItemUsageDirector
    {
        private IUserRetriever _userRetriever;
        private IItemRetriever _itemRetriever;
        private IUserItemSaver _userItemSaver;
        private IUserItemRetriever _userItemRetriever;
        private IUserSaver _userSaver;
        private IUserSightRadiusProvider _userSightRadiusProvider;
        private IUserEnergyProvider _userEnergyProvider;

        public ItemUsageDirector(IUserRetriever userRetriever, IItemRetriever itemRetriever, IUserItemSaver userItemSaver, IUserItemRetriever userItemRetriever, IUserSaver userSaver, IUserEnergyProvider userEnergyProvider, IUserSightRadiusProvider userSightRadiusProvider)
        {
            _userRetriever = userRetriever;
            _itemRetriever = itemRetriever;
            _userItemSaver = userItemSaver;
            _userItemRetriever = userItemRetriever;
            _userSaver = userSaver;
            _userEnergyProvider = userEnergyProvider;
            _userSightRadiusProvider = userSightRadiusProvider;
        }

        void IItemUsageDirector.UseItem(Guid userId, Guid itemId)
        {
            if (!_userRetriever.UserExists(userId))
            {
                return;
            }

            if (!_itemRetriever.ItemExists(itemId))
            {
                return;
            }

            IItem item = _itemRetriever.GetItemById(itemId);
            if (item.IsOneTimeUse == false)
            {
                return;
            }

            _userItemSaver.RemoveUserItem(_userItemRetriever.GetUserItemId(userId, itemId));

            int energyChange = item.Energy;
            if (energyChange != 0)
            {
                int currentEnergy = _userEnergyProvider.GetUserEnergy(userId, DateTime.Now);
                int maxEnergy = _userEnergyProvider.GetUserMaxEnergy(userId);
                currentEnergy += energyChange;
                if (currentEnergy > maxEnergy)
                {
                    currentEnergy = maxEnergy;
                }

                if (currentEnergy < 0)
                {
                    currentEnergy = 0;
                }

                _userSaver.SaveLastEnergy(userId, currentEnergy, DateTime.Now);
            }

            int distanceChange = item.Distance;
            if (distanceChange != 0)
            {
                int currentDistance = _userSightRadiusProvider.GetUserSightRadius(userId, DateTime.Now);
                int minDistance = _userSightRadiusProvider.GetUserMinSightRadius(userId);
                currentDistance += distanceChange;
                if (currentDistance < minDistance)
                {
                    currentDistance = minDistance;
                }

                _userSaver.SaveLastSightRadius(userId, currentDistance, DateTime.Now);
            }
        }
    }
}
