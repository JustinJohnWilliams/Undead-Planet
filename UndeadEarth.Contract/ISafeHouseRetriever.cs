using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface ISafeHouseRetriever
    {
        /// <summary>
        /// Returns a list of all safe houses within viewable radius
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        List<ISafeHouse> GetAllSafeHousesInRadius(double latitude, double longitude, double radius);

        /// <summary>
        /// Returns a list of items that the specified user has left in the safe house
        /// </summary>
        List<IItem> GetItems(Guid userId);

        /// <summary>
        /// Checks to see if id passed in is a valid safe house
        /// </summary>
        /// <param name="safeHouseId"></param>
        /// <returns></returns>
        bool SafeHouseExists(Guid safeHouseId);

        /// <summary>
        /// Checks to make sure item exists in safe house for user
        /// </summary>
        bool SafeHouseHasItem(Guid itemId, Guid userId);

        /// <summary>
        /// Returns the id of the item stored in the safe house for particular user
        /// </summary>
        /// <param name="safeHouseId"></param>
        /// <param name="itemId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Guid GetSafeHouseItemId(Guid itemId, Guid userId);

        /// <summary>
        /// Retrieves all safhouses by hotzone id.
        /// </summary>
        /// <param name="hotZoneId"></param>
        /// <returns></returns>
        List<ISafeHouse> GetAllSafeHousesByHotZoneId(Guid hotZoneId);
    }
}
