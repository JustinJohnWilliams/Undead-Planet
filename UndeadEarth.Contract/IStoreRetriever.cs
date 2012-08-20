using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IStoreRetriever
    {
        /// <summary>
        /// Returns a list of all stores
        /// </summary>
        /// <returns></returns>
        List<IStore> GetAllStores();

        /// <summary>
        /// Returns a list of all stores within users viewable radius.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        List<IStore> GetAllStoresInRadius(double latitude, double longitude, double radius);

        /// <summary>
        /// Returns true/false if store exists at lat/long position
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        bool StoreExists(double latitude, double longitude);

        /// <summary>
        /// Returns a list of items the store has in its inventory for the user to buy
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        List<IItem> GetItems(Guid storeId);

        /// <summary>
        /// Retrieves all safhouses by hotzone id.
        /// </summary>
        /// <param name="hotZoneId"></param>
        /// <returns></returns>
        List<IStore> GetAllStoresByHotZoneId(Guid hotZoneId);
    }
}
