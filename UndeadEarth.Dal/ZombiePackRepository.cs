using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using UndeadEarth.Dal.Database;

namespace UndeadEarth.Dal
{
    public class ZombiePackRepository : IZombiePackRetriever
    {
        private string _connectionString;
        private IDistanceCalculator _distanceCalculator;
        
        public ZombiePackRepository(string connectionString, IDistanceCalculator distanceCalculator)
        {
            _connectionString = connectionString;
            _distanceCalculator = distanceCalculator;
        }

        IZombiePack IZombiePackRetriever.GetZombiePackById(Guid id)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.ZombiePackDtos.SingleOrDefault(c => c.Id == id);
            }
        }

        List<IZombiePack> IZombiePackRetriever.GetAllZombiePacksInRadius(double latitude, double longitude, double radius)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                Tuple<double, double> north = _distanceCalculator.GetNorthernPoint(latitude, longitude, radius);
                Tuple<double, double> south = _distanceCalculator.GetSouthernPoint(latitude, longitude, radius);
                Tuple<double, double> east = _distanceCalculator.GetEasternPoint(latitude, longitude, radius);
                Tuple<double, double> west = _distanceCalculator.GetWesternPoint(latitude, longitude, radius);

                List<IZombiePack> zombiePacks = dataContext.ZombiePackDtos.Where(c => (double)c.Latitude < north.Item1 && (double)c.Latitude > south.Item1
                                                                && (double)c.Longitude < east.Item2 && (double)c.Longitude > west.Item2)
                                                                .Cast<IZombiePack>().ToList();

                List<IZombiePack> zombiePacksToRemove = new List<IZombiePack>();

                foreach (IZombiePack zombiePack in zombiePacks)
                {
                    if (_distanceCalculator.CalculateMiles(latitude, longitude, zombiePack.Latitude, zombiePack.Longitude) > radius)
                    {
                        zombiePacksToRemove.Add(zombiePack);
                    }
                }

                zombiePacks.RemoveAll(c => zombiePacksToRemove.Contains(c));

                return zombiePacks;
            }
        }

        Guid IZombiePackRetriever.GetHotZoneByZombiePackId(Guid zombiePackId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.ZombiePackDtos.Single(c => c.Id == zombiePackId).HotZoneId;
            }
        }

        bool IZombiePackRetriever.ZombiePackExists(Guid zombiePackId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.ZombiePackDtos.Any(c => c.Id == zombiePackId);
            }
        }

        List<IZombiePack> IZombiePackRetriever.GetAllZombiePacksInHotZone(Guid hotZoneId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.ZombiePackDtos.Where(c => c.HotZoneId == hotZoneId).Cast<IZombiePack>().ToList();
            }
        }
    }
}
