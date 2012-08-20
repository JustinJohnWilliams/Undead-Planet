using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using UndeadEarth.Test.Dal.Utility;
using UndeadEarth.Contract;
using Moq;
using UndeadEarth.Dal;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_retrieving_zombie_packs
    {
        private string _connectionString;
        private double _radiusToUse;

        private Guid _hotZoneId;

        private Guid _zombiePackId;
        private double _latitudeToUse;
        private double _longitudeToUse;

        private Guid _zombiePackId2;
        private double _latitudeToUse2;
        private double _longitudeToUse2;

        IZombiePackRetriever _zombiePackRetriever;
        Mock<IDistanceCalculator> _distanceCalculator;

        public when_retrieving_zombie_packs()
        {
            _connectionString = DalTestContextSpecification.ConnectionString;
            _radiusToUse = 5.0;

            _hotZoneId = Guid.NewGuid();

            _zombiePackId = Guid.NewGuid();
            _latitudeToUse = 0;
            _longitudeToUse = 0;

            _zombiePackId2 = Guid.NewGuid();
            _latitudeToUse2 = 1;
            _longitudeToUse2 = 1;

            _distanceCalculator = new Mock<IDistanceCalculator>();
            _zombiePackRetriever = new ZombiePackRepository(_connectionString, _distanceCalculator.Object);
        }


        [TestInitialize()]
        public void BecauseOf()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                List<HotZoneDto> hotZoneDtos = new List<HotZoneDto>();
                hotZoneDtos.Add(new HotZoneDto
                {
                    Id = _hotZoneId,
                    Latitude = (decimal)_latitudeToUse,
                    Longitude = (decimal)_longitudeToUse,
                    Name = "Hot Zone"
                });

                dataContext.HotZoneDtos.InsertAllOnSubmit(hotZoneDtos);
                dataContext.SubmitChanges();

                List<ZombiePackDto> zombiePackDtos = new List<ZombiePackDto>();
                zombiePackDtos.Add(new ZombiePackDto
                {
                    HotZoneId = _hotZoneId,
                    Id = _zombiePackId,
                    Latitude = (decimal)_latitudeToUse,
                    Longitude = (decimal)_longitudeToUse,
                    Name = "Zombie Pack1"
                });

                zombiePackDtos.Add(new ZombiePackDto
                {
                    HotZoneId = _hotZoneId,
                    Id = _zombiePackId2,
                    Latitude = (decimal)_latitudeToUse2,
                    Longitude = (decimal)_longitudeToUse2,
                    Name = "Zombie Pack2"
                });

                dataContext.ZombiePackDtos.InsertAllOnSubmit(zombiePackDtos);
                dataContext.SubmitChanges();
            }
        }

        [TestCleanup()]
        public void CleanUp()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                dataContext.HotZoneDtos.DeleteAllOnSubmit(
                                dataContext.HotZoneDtos.Where(c => c.Id == _hotZoneId));

                dataContext.ZombiePackDtos.DeleteAllOnSubmit(
                                dataContext.ZombiePackDtos.Where(c => c.Id == _zombiePackId
                                                                    || c.Id == _zombiePackId2));

                dataContext.SubmitChanges();
            }
        }

        [TestMethod]
        public void should_retrieve_zombie_pack_by_id()
        {
            IZombiePack zombiePack = _zombiePackRetriever.GetZombiePackById(_zombiePackId);

            Assert.AreEqual(_hotZoneId, zombiePack.HotZoneId);
            Assert.AreEqual(_zombiePackId, zombiePack.Id);
            Assert.AreEqual(_latitudeToUse, zombiePack.Latitude);
            Assert.AreEqual(_longitudeToUse, zombiePack.Longitude);
            Assert.AreEqual("Zombie Pack1", zombiePack.Name);
        }

        [TestMethod]
        public void should_return_null_if_zombie_pack_does_not_exist()
        {
            IZombiePack zombiePack = _zombiePackRetriever.GetZombiePackById(Guid.NewGuid());

            Assert.IsNull(zombiePack);
        }

        [TestMethod]
        public void should_return_all_zombie_packs_in_radius()
        {
            _distanceCalculator.Setup(
                        s => s.GetEasternPoint(_latitudeToUse, _longitudeToUse, _radiusToUse))
                              .Returns(new UndeadEarth.Contract.Tuple<double, double>(0, 3));
            _distanceCalculator.Setup(
                        s => s.GetNorthernPoint(_latitudeToUse, _longitudeToUse, _radiusToUse))
                              .Returns(new UndeadEarth.Contract.Tuple<double, double>(3, 0));
            _distanceCalculator.Setup(
                        s => s.GetWesternPoint(_latitudeToUse, _longitudeToUse, _radiusToUse))
                              .Returns(new UndeadEarth.Contract.Tuple<double, double>(0, -3));
            _distanceCalculator.Setup(
                        s => s.GetSouthernPoint(_latitudeToUse, _longitudeToUse, _radiusToUse))
                              .Returns(new UndeadEarth.Contract.Tuple<double, double>(-3, 0));

            _distanceCalculator.Setup(c => c.CalculateMiles(_latitudeToUse, _longitudeToUse, _latitudeToUse2, _longitudeToUse2)).Returns(10);

            List<IZombiePack> zombiePacks = _zombiePackRetriever.GetAllZombiePacksInRadius(_latitudeToUse, _longitudeToUse, _radiusToUse)
                                            .OrderBy(c => c.Name).Cast<IZombiePack>().ToList();

            Assert.AreEqual(1, zombiePacks.Count());

            Assert.AreEqual(_hotZoneId, zombiePacks.ElementAt(0).HotZoneId);
            Assert.AreEqual(_zombiePackId, zombiePacks.ElementAt(0).Id);
            Assert.AreEqual(_latitudeToUse, zombiePacks.ElementAt(0).Latitude);
            Assert.AreEqual(_longitudeToUse, zombiePacks.ElementAt(0).Longitude);
            Assert.AreEqual("Zombie Pack1", zombiePacks.ElementAt(0).Name);
        }

        [TestMethod]
        public void should_return_all_zombie_packs_assocaited_with_hotzone()
        {
            var zombiePacks = _zombiePackRetriever.GetAllZombiePacksInHotZone(_hotZoneId);
            Assert.AreEqual(2, zombiePacks.Count);
        }

        [TestMethod]
        public void should_return_hotzone_id_containing_zombie_pack()
        {
            Guid hotZoneId = _zombiePackRetriever.GetHotZoneByZombiePackId(_zombiePackId);
            Guid hotZoneId2 = _zombiePackRetriever.GetHotZoneByZombiePackId(_zombiePackId2);

            Assert.AreEqual(_hotZoneId, hotZoneId);
            Assert.AreEqual(_hotZoneId, hotZoneId2);
        }
    }
}
