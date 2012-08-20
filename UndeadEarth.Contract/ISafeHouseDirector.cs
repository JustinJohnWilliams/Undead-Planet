using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface ISafeHouseDirector
    {
        /// <summary>
        /// Transfers an item from the user to a safe house for storage
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="safeHouseId"></param>
        /// <param name="itemId"></param>
        void TransferItemFromUserToSafeHouse(Guid userId, Guid safeHouseId, Guid itemId);

        /// <summary>
        /// Transfers an item from the safe house to a user for user storage
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="safeHouseId"></param>
        /// <param name="itemId"></param>
        void TransferItemFromSafeHouseToUser(Guid userId, Guid safeHouseId, Guid itemId);
    }
}
