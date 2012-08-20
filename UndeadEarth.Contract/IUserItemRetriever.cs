using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserItemRetriever
    {
        /// <summary>
        /// Gets a list of the items the user posesses
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<IItem> GetUserItems(Guid userId);

        /// <summary>
        /// Returns whether or not the specified user has the specified item
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        bool UserHasItem(Guid userId, Guid itemId);

        /// <summary>
        /// Returns the useritem id that corresponds to given item and user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        Guid GetUserItemId(Guid userId, Guid itemId);

        /// <summary>
        /// Returns the count of items the user has
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetUserItemCount(Guid userId);
    }
}
