using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserItemSaver
    {
        /// <summary>
        /// Saves item in user inventory
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="itemId"></param>
        void SaveUserItem(Guid userId, Guid itemId);

        /// <summary>
        /// Adds a tent to the user's inventory.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="itemId"></param>
        void AddTent(Guid userId, int quantity);

        /// <summary>
        /// Removes specified item from user's inventory
        /// </summary>
        /// <param name="userItemId"></param>
        void RemoveUserItem(Guid userItemId);
    }
}
