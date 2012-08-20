using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface ISafeHouseItemSaver
    {
        /// <summary>
        /// Saves item from user in safe house for storage
        /// </summary>
        /// <param name="safeHouseId"></param>
        /// <param name="itemId"></param>
        /// <param name="userId"></param>
        void SaveItemInSafeHouse(Guid safeHouseId, Guid itemId, Guid userId);

        /// <summary>
        /// Removes the item from the safe house that belongs to the user
        /// </summary>
        /// <param name="safeHouseUserItemId"></param>
        void RemoveSafeHouseItem(Guid safeHouseUserItemId);
    }
}
