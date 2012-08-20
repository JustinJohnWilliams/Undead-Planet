using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using UndeadEarth.Dal.Database;

namespace UndeadEarth.Dal
{
    public class SafeHouseRepository : ISafeHouseRetriever
    {
        private string _connectionString;
        private IDistanceCalculator _distanceCalculator;

        public SafeHouseRepository(string connectionString, IDistanceCalculator distanceCalculator)
        {
            _connectionString = connectionString;
            _distanceCalculator = distanceCalculator;
        }

        List<ISafeHouse> ISafeHouseRetriever.GetAllSafeHousesInRadius(double latitude, double longitude, double radius)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                Tuple<double, double> north = _distanceCalculator.GetNorthernPoint(latitude, longitude, radius);
                Tuple<double, double> south = _distanceCalculator.GetSouthernPoint(latitude, longitude, radius);
                Tuple<double, double> east = _distanceCalculator.GetEasternPoint(latitude, longitude, radius);
                Tuple<double, double> west = _distanceCalculator.GetWesternPoint(latitude, longitude, radius);

                List<ISafeHouse> safeHouses = dataContext.SafeHouseDtos.Where(c => (double)c.Latitude < north.Item1 && (double)c.Latitude > south.Item1
                                                                                && (double)c.Longitude < east.Item2 && (double)c.Longitude > west.Item2)
                                                                                .Cast<ISafeHouse>().ToList();
                List<ISafeHouse> safeHousesToRemove = new List<ISafeHouse>();

                foreach (ISafeHouse safeHouse in safeHouses)
                {
                    if (_distanceCalculator.CalculateMiles(latitude, longitude, safeHouse.Latitude, safeHouse.Longitude) > radius)
                    {
                        safeHousesToRemove.Add(safeHouse);
                    }
                }

                safeHouses.RemoveAll(c => safeHousesToRemove.Contains(c));

                return safeHouses;
            }
        }

        List<IItem> ISafeHouseRetriever.GetItems(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                var items = (from safeHouseItem in dataContext.SafeHouseItemDtos
                             join item in dataContext.ItemDtos
                             on safeHouseItem.ItemId equals item.Id
                             where safeHouseItem.UserId == userId
                             select item);

                return items.Cast<IItem>().ToList(); ;
            }
        }

        bool ISafeHouseRetriever.SafeHouseExists(Guid safeHouseId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.SafeHouseDtos.Any(c => c.Id == safeHouseId);
            }
        }

        bool ISafeHouseRetriever.SafeHouseHasItem(Guid itemId, Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.SafeHouseItemDtos.Any(c => c.ItemId == itemId
                                                            && c.UserId == userId);
            }
        }

        Guid ISafeHouseRetriever.GetSafeHouseItemId(Guid itemId, Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.SafeHouseItemDtos.First(c => c.ItemId == itemId
                                                                && c.UserId == userId).Id;
            }
        }

        List<ISafeHouse> ISafeHouseRetriever.GetAllSafeHousesByHotZoneId(Guid hotZoneId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.SafeHouseDtos.Where(s => s.HotZoneId == hotZoneId).Cast<ISafeHouse>().ToList();
            }
        }
    }
}
