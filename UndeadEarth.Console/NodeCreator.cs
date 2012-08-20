using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Console
{
    public class NodeCreator : INodeCreator
    {
        private string _connectionString;
        private IDistanceCalculator _distanceCalculator;
        private IRandomNumberProvider _randomNumberProvider;

        public NodeCreator(string connectionString, IDistanceCalculator distanceCalculator, IRandomNumberProvider randomNumberProvider)
        {
            _connectionString = connectionString;
            _distanceCalculator = distanceCalculator;
            _randomNumberProvider = randomNumberProvider;
        }

        void INodeCreator.CreateZombiePacks()
        {
            double radius = 100;

            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                List<ZombiePackDto> zombiePacks = new List<ZombiePackDto>();

                foreach (HotZoneDto hotZoneDto in dataContext.HotZoneDtos)
                {
                    //if (hotZoneDto.Id != new Guid("7270330C-F873-4452-9ACE-D1E6BA1A74F6") && hotZoneDto.Id != new Guid("81ABC8B3-E1EB-48F6-9643-3F3C1D064132"))
                    //{
                        UndeadEarth.Contract.Tuple<double, double> north = _distanceCalculator.GetNorthernPoint((double)hotZoneDto.Latitude, (double)hotZoneDto.Longitude, radius);
                        UndeadEarth.Contract.Tuple<double, double> south = _distanceCalculator.GetSouthernPoint((double)hotZoneDto.Latitude, (double)hotZoneDto.Longitude, radius);
                        UndeadEarth.Contract.Tuple<double, double> east = _distanceCalculator.GetEasternPoint((double)hotZoneDto.Latitude, (double)hotZoneDto.Longitude, radius);
                        UndeadEarth.Contract.Tuple<double, double> west = _distanceCalculator.GetWesternPoint((double)hotZoneDto.Latitude, (double)hotZoneDto.Longitude, radius);

                        for (int i = 0; i < 100; i++)
                        {
                            UndeadEarth.Contract.Tuple<double, double> points = GetRandomPoint(north, south, east, west);

                            var zombiePack = new ZombiePackDto();
                            zombiePack.Id = Guid.NewGuid();
                            zombiePack.HotZoneId = hotZoneDto.Id;
                            zombiePack.Name = string.Format("Zombie Pack: {0}", i);
                            zombiePack.Latitude = (decimal)points.Item1;
                            zombiePack.Longitude = (decimal)points.Item2;

                            zombiePacks.Add(zombiePack);

                        }
                    //}

                    dataContext.ZombiePackDtos.InsertAllOnSubmit(zombiePacks);
                    dataContext.SubmitChanges();

                    zombiePacks.Clear();
                }
            }
        }

        void INodeCreator.CreateShops()
        {
            double radius = 5;

            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                List<StoreDto> storeDtos = new List<StoreDto>();

                foreach (HotZoneDto hotZoneDto in dataContext.HotZoneDtos)
                {
                    UndeadEarth.Contract.Tuple<double, double> north = _distanceCalculator.GetNorthernPoint((double)hotZoneDto.Latitude, (double)hotZoneDto.Longitude, radius);
                    UndeadEarth.Contract.Tuple<double, double> south = _distanceCalculator.GetSouthernPoint((double)hotZoneDto.Latitude, (double)hotZoneDto.Longitude, radius);
                    UndeadEarth.Contract.Tuple<double, double> east = _distanceCalculator.GetEasternPoint((double)hotZoneDto.Latitude, (double)hotZoneDto.Longitude, radius);
                    UndeadEarth.Contract.Tuple<double, double> west = _distanceCalculator.GetWesternPoint((double)hotZoneDto.Latitude, (double)hotZoneDto.Longitude, radius);

                    UndeadEarth.Contract.Tuple<double, double> points = GetRandomPoint(north, south, east, west);

                    var shop = new StoreDto();
                    shop.Id = Guid.NewGuid();
                    shop.HotZoneId = hotZoneDto.Id;
                    shop.Name = string.Format("Shop at {0}", hotZoneDto.Name);
                    shop.Latitude = (decimal)points.Item1;
                    shop.Longitude = (decimal)points.Item2;

                    storeDtos.Add(shop);

                }

                dataContext.StoreDtos.InsertAllOnSubmit(storeDtos);
                dataContext.SubmitChanges();

                storeDtos.Clear();
            }
        }

        void INodeCreator.CreateSafeHouses()
        {
            double radius = 100;

            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                List<SafeHouseDto> safeHouses = new List<SafeHouseDto>();

                foreach (HotZoneDto hotZoneDto in dataContext.HotZoneDtos)
                {
                    UndeadEarth.Contract.Tuple<double, double> north = _distanceCalculator.GetNorthernPoint((double)hotZoneDto.Latitude, (double)hotZoneDto.Longitude, radius);
                    UndeadEarth.Contract.Tuple<double, double> south = _distanceCalculator.GetSouthernPoint((double)hotZoneDto.Latitude, (double)hotZoneDto.Longitude, radius);
                    UndeadEarth.Contract.Tuple<double, double> east = _distanceCalculator.GetEasternPoint((double)hotZoneDto.Latitude, (double)hotZoneDto.Longitude, radius);
                    UndeadEarth.Contract.Tuple<double, double> west = _distanceCalculator.GetWesternPoint((double)hotZoneDto.Latitude, (double)hotZoneDto.Longitude, radius);

                    var shopInHotZone = dataContext.StoreDtos.First(c => c.HotZoneId == hotZoneDto.Id);

                    SafeHouseDto safeHouseNextToStore = new SafeHouseDto
                    {
                        HotZoneId = hotZoneDto.Id,
                        Id = Guid.NewGuid(),
                        Latitude = (shopInHotZone.Latitude + (decimal).1),
                        Longitude = (shopInHotZone.Longitude - (decimal).1)
                    };

                    safeHouses.Add(safeHouseNextToStore);

                    for (int i = 0; i < 4; i++)
                    {
                        UndeadEarth.Contract.Tuple<double, double> points = GetRandomPoint(north, south, east, west);

                        var safeHouse = new SafeHouseDto();
                        safeHouse.Id = Guid.NewGuid();
                        safeHouse.HotZoneId = hotZoneDto.Id;
                        safeHouse.Latitude = (decimal)points.Item1;
                        safeHouse.Longitude = (decimal)points.Item2;

                        safeHouses.Add(safeHouse);
                    }
                }

                dataContext.SafeHouseDtos.InsertAllOnSubmit(safeHouses);
                dataContext.SubmitChanges();

                safeHouses.Clear();
            }
        }

        void INodeCreator.PurgeShops()
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.StoreDtos.DeleteAllOnSubmit(dataContext.StoreDtos);
                dataContext.SubmitChanges();
            }
        }

        void INodeCreator.PurgeSafeHouses()
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.SafeHouseDtos.DeleteAllOnSubmit(dataContext.SafeHouseDtos);
                dataContext.SubmitChanges();
            }
        }

        void INodeCreator.PurgeZombiePacks()
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ZombiePackDtos.DeleteAllOnSubmit(dataContext.ZombiePackDtos);
                dataContext.SubmitChanges();
            }
        }

        private UndeadEarth.Contract.Tuple<double, double> GetRandomPoint(UndeadEarth.Contract.Tuple<double, double> north
                                                                        , UndeadEarth.Contract.Tuple<double, double> south
                                                                        , UndeadEarth.Contract.Tuple<double, double> east
                                                                        , UndeadEarth.Contract.Tuple<double, double> west)
        {
            int someLatPoint = _randomNumberProvider.GetRandom(Convert.ToInt32(Math.Round(south.Item1)), Convert.ToInt32(Math.Round(north.Item1)));

            int someLongPoint = _randomNumberProvider.GetRandom(Convert.ToInt32(Math.Round(west.Item2)), Convert.ToInt32(Math.Round(east.Item2)));

            double someAddedValueForLatDecimalPlaces = _randomNumberProvider.GetNextDouble();
            double someAddedValueForLongDecimalPlaces = _randomNumberProvider.GetNextDouble();

            double lattitude = someLatPoint + someAddedValueForLatDecimalPlaces;
            double longitude = someLongPoint + someAddedValueForLongDecimalPlaces;

            return new UndeadEarth.Contract.Tuple<double, double>(lattitude, longitude);
        }



    }
}
