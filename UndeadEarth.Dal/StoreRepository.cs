using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using UndeadEarth.Dal.Database;

namespace UndeadEarth.Dal
{
    public class StoreRepository : IStoreRetriever
    {
        private string _connectionString;
        private IDistanceCalculator _distanceCalculator;

        public StoreRepository(string connectionString, IDistanceCalculator distanceCalculator)
        {
            _connectionString = connectionString;
            _distanceCalculator = distanceCalculator;
        }
        #region IStoreRetriever Members

        List<IStore> IStoreRetriever.GetAllStores()
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.StoreDtos.Cast<IStore>().ToList();
            }
        }

        List<IStore> IStoreRetriever.GetAllStoresInRadius(double latitude, double longitude, double radius)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                Tuple<double, double> north = _distanceCalculator.GetNorthernPoint(latitude, longitude, radius);
                Tuple<double, double> south = _distanceCalculator.GetSouthernPoint(latitude, longitude, radius);
                Tuple<double, double> east = _distanceCalculator.GetEasternPoint(latitude, longitude, radius);
                Tuple<double, double> west = _distanceCalculator.GetWesternPoint(latitude, longitude, radius);

                List<IStore> stores = dataContext.StoreDtos.Where(c => (double)c.Latitude < north.Item1 && (double)c.Latitude > south.Item1
                                                                && (double)c.Longitude < east.Item2 && (double)c.Longitude > west.Item2)
                                                                .Cast<IStore>().ToList();

                List<IStore> storesToRemove = new List<IStore>();

                foreach (IStore store in stores)
                {
                    if (_distanceCalculator.CalculateMiles(latitude, longitude, store.Latitude, store.Longitude) > radius)
                    {
                        storesToRemove.Add(store);
                    }
                }

                stores.RemoveAll(c => storesToRemove.Contains(c));

                return stores;
            }
        }

        bool IStoreRetriever.StoreExists(double latitude, double longitude)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.StoreDtos.Any(c => (double)c.Latitude == latitude && (double)c.Longitude == longitude);
            }
        }

        List<IItem> IStoreRetriever.GetItems(Guid storeId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.ItemDtos.Cast<IItem>().ToList();
            }
        }

        List<IStore> IStoreRetriever.GetAllStoresByHotZoneId(Guid hotZoneId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.StoreDtos.Where(s => s.HotZoneId == hotZoneId).Cast<IStore>().ToList();
            }
        }

        #endregion
    }
}
