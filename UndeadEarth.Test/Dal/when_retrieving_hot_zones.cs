using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Test.Dal.Utility;
using UndeadEarth.Contract;
using System.Configuration;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_retrieving_hot_zones
    {
        private string _connectionString;

        private IHotZoneRetriever _hotZoneRetriever;

        private string _displayName;
        private Guid _idToUse;
        private double _lattitude;
        private double _longitude;

        private Guid _zombiePackId1;
        private double _zombiePackLat1;
        private double _zombiePackLong1;
        private string _zombiePackName1;

        private string _displayName2;
        private Guid _idToUse2;
        private double _lattitude2;
        private double _longitude2;

        private Guid _zombiePackId2;
        private double _zombiePackLong2;
        private string _zombiePackName2;

        private Guid _userId;

        private int _dbCount;

        public when_retrieving_hot_zones()
        {
            _connectionString = DalTestContextSpecification.ConnectionString;

            _hotZoneRetriever = DalTestContextSpecification.Instance.Resolve<IHotZoneRetriever>();

            _displayName = "DisplayName";
            _idToUse = Guid.NewGuid();
            _lattitude = 1234.5678;
            _longitude = 5678.1234;

            _zombiePackId1 = Guid.NewGuid();
            _zombiePackLat1 = 1234.5679;
            _zombiePackLong1 = 5678.1235;
            _zombiePackName1 = "ZombiePack1";

            _displayName2 = "DisplayName2";
            _idToUse2 = Guid.NewGuid();
            _lattitude2 = 1111.9999;
            _longitude2 = 9999.1111;

            _zombiePackId2 = Guid.NewGuid();
            _zombiePackLong2 = 9999.1110;
            _zombiePackName2 = "ZombiePack2";

            _userId = Guid.NewGuid();
        }

        [TestInitialize()]
        public void BeacauseOf()
        {
            using (TestDataContext dataConext = new TestDataContext(_connectionString))
            {
                List<HotZoneDto> hotZoneDtos = new List<HotZoneDto>();
                hotZoneDtos.Add(new HotZoneDto
                {
                    Id = _idToUse,
                    Latitude = (decimal)_lattitude,
                    Longitude = (decimal)_longitude,
                    Name = _displayName,
                    CanStartHere = true
                });

                hotZoneDtos.Add(new HotZoneDto
                {
                    Id = _idToUse2,
                    Latitude = (decimal)_lattitude2,
                    Longitude = (decimal)_longitude2,
                    Name = _displayName2
                });

                dataConext.HotZoneDtos.InsertAllOnSubmit(hotZoneDtos);
                dataConext.SubmitChanges();

                List<ZombiePackDto> zombiePackDtos = new List<ZombiePackDto>();
                zombiePackDtos.Add(new ZombiePackDto
                {
                    Id = _zombiePackId1,
                    Latitude = (decimal)_zombiePackLat1,
                    Longitude = (decimal)_zombiePackLong1,
                    Name = _zombiePackName1,
                    HotZoneId = _idToUse
                });

                zombiePackDtos.Add(new ZombiePackDto
                {
                    Id = _zombiePackId2,
                    Latitude = (decimal)_zombiePackLat1,
                    Longitude = (decimal)_zombiePackLong2,
                    Name = _zombiePackName2,
                    HotZoneId = _idToUse
                });

                dataConext.ZombiePackDtos.InsertAllOnSubmit(zombiePackDtos);
                dataConext.SubmitChanges();

                List<UserDto> userDtos = new List<UserDto>();
                userDtos.Add(new UserDto
                {
                    Id = _userId,
                    DisplayName = "Display Name",
                    ZoneId = _idToUse,
                    Latitude = (decimal)_lattitude,
                    Longitude = (decimal)_longitude,
                    LocationId = _idToUse,
                    Email = "Test@Test.com"
                });

                dataConext.UserDtos.InsertAllOnSubmit(userDtos);
                dataConext.SubmitChanges();

                _dbCount = dataConext.HotZoneDtos.Count();

            }
        }

        [TestCleanup()]
        public void CleanUp()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                dataContext.HotZoneDtos.DeleteAllOnSubmit(
                            dataContext.HotZoneDtos.Where(c => c.Id == _idToUse
                                                            || c.Id == _idToUse2));

                dataContext.ZombiePackDtos.DeleteAllOnSubmit(
                            dataContext.ZombiePackDtos.Where(c => c.Id == _zombiePackId1
                                                                || c.Id == _zombiePackId2));

                dataContext.UserDtos.DeleteAllOnSubmit(
                            dataContext.UserDtos.Where(c => c.Id == _userId));

                dataContext.SubmitChanges();
            }
        }

        [TestMethod]
        public void should_return_all_hot_zones()
        {
            List<IHotZone> hotZones = _hotZoneRetriever.GetAllHotZones();

            IHotZone hotZone1 = hotZones.SingleOrDefault(c => c.Id == _idToUse);
            Assert.AreEqual(_idToUse, hotZone1.Id);
            Assert.AreEqual(_displayName, hotZone1.Name);
            Assert.AreEqual(_lattitude, hotZone1.Latitude);
            Assert.AreEqual(_longitude, hotZone1.Longitude);

            IHotZone hotZone2 = hotZones.SingleOrDefault(c => c.Id == _idToUse2);
            Assert.AreEqual(_idToUse2, hotZone2.Id);
            Assert.AreEqual(_displayName2, hotZone2.Name);
            Assert.AreEqual(_lattitude2, hotZone2.Latitude);
            Assert.AreEqual(_longitude2, hotZone2.Longitude);

            Assert.AreEqual<int>(_dbCount, hotZones.Count);
        }

        [TestMethod]
        public void should_return_number_of_zombie_packs_left()
        {
            int count = _hotZoneRetriever.ZombiePacksLeft(_userId, _idToUse);

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void should_return_remaining_zombie_packs_in_hot_zones()
        {
            List<KeyValuePair<Guid, int>> zombiePacksInHotZones = _hotZoneRetriever.GetRemainingZombiePacksInHotZones(_userId);

            Assert.IsTrue(zombiePacksInHotZones.Contains(new KeyValuePair<Guid,int>(_idToUse, 2)));
        }

        [TestMethod]
        public void should_return_false_if_hotzone_doesnt_exists()
        {
            Assert.IsFalse(_hotZoneRetriever.HotZoneExists(Guid.NewGuid()));
        }

        [TestMethod]
        public void should_return_true_if_hotzone_exists()
        {
            Assert.IsTrue(_hotZoneRetriever.HotZoneExists(_idToUse));
        }

        [TestMethod]
        public void should_return_true_if_hotzone_is_a_starting_hotzone()
        {
            Assert.IsTrue(_hotZoneRetriever.IsStartingHotZone(_idToUse));
        }

        [TestMethod]
        public void should_return_false_for_IsStartingHotzone_if_hotzone_doesnt_exist()
        {
            Assert.IsFalse(_hotZoneRetriever.IsStartingHotZone(Guid.NewGuid()));
        }

        [TestMethod]
        public void should_return_false_if_hotzone_is_not_a_starting_hotzone()
        {
            Assert.IsFalse(_hotZoneRetriever.IsStartingHotZone(_idToUse2));
        }

        [TestMethod]
        public void should_return_hotzones_that_are_marked_as_starting_hotzones()
        {
            List<KeyValuePair<Guid, string>> startingHotZones = _hotZoneRetriever.GetStartingHotZones();
            Assert.IsTrue(startingHotZones.Any(s => s.Key == _idToUse && s.Value == _displayName));
        }

        [TestMethod]
        public void should_retrieve_single_hotzone()
        {
            IHotZone hotzone = _hotZoneRetriever.GetHotZone(_idToUse);
            Assert.AreEqual(_idToUse, hotzone.Id);
            Assert.AreEqual(_displayName, hotzone.Name);
            Assert.AreEqual(_lattitude, hotzone.Latitude);
            Assert.AreEqual(_longitude, hotzone.Longitude);
        }

        [TestMethod]
        public void should_return_null_if_single_hotzone_doesnt_exist()
        {
            Assert.IsNull(_hotZoneRetriever.GetHotZone(Guid.NewGuid()));
        }
    }
}
