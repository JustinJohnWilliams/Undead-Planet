using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IItemRetriever
    {
        /// <summary>
        /// Returns item or null if user does not exist
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        IItem GetItemById(Guid itemId);

        /// <summary>
        /// Checks to see if the id passed in is a valid item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        bool ItemExists(Guid itemId);


        /// <summary>
        /// Returns all items at or below the specified price.
        /// </summary>
        List<IItem> GetAllBelowPrice(int price);
    }
}
