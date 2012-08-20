using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Test.Dal.Utility;
using UndeadEarth.Contract;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_retrieving_user_zombie_pack_progress
    {
        private string _connectionString;

        private IUserZombiePackProgressRetriever _userZombiePackProgressRetriever;

        private Guid _userId;

        private Guid _hotZoneId;
        private double _latitude;
        private double _longitude;

        private Guid _hotZoneId2;
        private double _latitude2;
        private double _longitude2;

        private Guid _zombiePackId1;
        private double _zombiePackLat1;
        private double _zombiePackLong1;

        private Guid _zombiePackId2;
        private double _zombiePackLong2;

        private Guid _userZombiePackProgressId;
        private Guid _userZombiePackProgressId2;

        public when_retrieving_user_zombie_pack_progress()
        {
            _connectionString = DalTestContextSpecification.ConnectionString;

            _userZombiePackProgressRetriever = DalTestContextSpecification.Instance.Resolve<IUserZombiePackProgressRetriever>();

            _userId = Guid.NewGuid();

            _hotZoneId = Guid.NewGuid();
            _latitude = 1234.5678;
            _longitude = 5678.1234;

            _zombiePackId1 = Guid.NewGuid();
            _zombiePackLat1 = 1234.5679;
            _zombiePackLong1 = 5678.1235;

            _hotZoneId2 = Guid.NewGuid();
            _latitude2 = 1111.9999;
            _longitude2 = 9999.1111;

            _zombiePackId2 = Guid.NewGuid();
            _zombiePackLong2 = 9999.1110;

            _userZombiePackProgressId = Guid.NewGuid();
            _userZombiePackProgressId2 = Guid.NewGuid();
        }

        [TestInitialize()]
        public void BecauseOf()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                List<UserDto> userDtos = new List<UserDto>();
                userDtos.Add(new UserDto
                {
                    Id = _userId,
                    DisplayName = "Display Name",
                    ZoneId = _hotZoneId,
                    Latitude = (decimal)_latitude,
                    Longitude = (decimal)_longitude,
                    LocationId = _hotZoneId,
                    Email = "Test@Test.com"
                });

                dataContext.UserDtos.InsertAllOnSubmit(userDtos);
                dataContext.SubmitChanges();

                List<ZombiePackDto> zombiePackDtos = new List<ZombiePackDto>();
                zombiePackDtos.Add(new ZombiePackDto
                {
                    HotZoneId = _hotZoneId,
                    Id = _zombiePackId1,
                    Latitude = (decimal)_zombiePackLat1,
                    Longitude = (decimal)_zombiePackLong1,
                    Name = "Zombie Pack 1"
                });

                zombiePackDtos.Add(new ZombiePackDto
                {
                    HotZoneId = _hotZoneId2,
                    Id = _zombiePackId2,
                    Latitude = (decimal)_zombiePackLat1,
                    Longitude = (decimal)_zombiePackLong2,
                    Name = "Zombie Pack 2"
                });

                dataContext.ZombiePackDtos.InsertAllOnSubmit(zombiePackDtos);
                dataContext.SubmitChanges();

                List<HotZoneDto> hotZoneDtos = new List<HotZoneDto>();
                hotZoneDtos.Add(new HotZoneDto
                {
                    Id = _hotZoneId,
                    Latitude = (decimal)_latitude,
                    Longitude = (decimal)_longitude,
                    Name = "Hot Zone 1"
                });

                hotZoneDtos.Add(new HotZoneDto
                {
                    Id = _hotZoneId2,
                    Latitude = (decimal)_latitude2,
                    Longitude = (decimal)_longitude2,
                    Name = "Hot Zone 2"
                });

                dataContext.HotZoneDtos.InsertAllOnSubmit(hotZoneDtos);
                dataContext.SubmitChanges();

                List<UserZombiePackProgressDto> userZombiePackProgressDtos = new List<UserZombiePackProgressDto>();
                userZombiePackProgressDtos.Add(new UserZombiePackProgressDto
                {
                    Id = _userZombiePackProgressId,
                    IsDestroyed = false,
                    LastHuntDate = null,
                    LastRegen = DateTime.Today.AddMinutes(-1),
                    MaxZombies = 100,
                    RegenMinuteTicks = 10,
                    RegenZombieRate = 5,
                    UserId = _userId,
                    ZombiePackId = _hotZoneId,
                    ZombiesLeft = 100
                });

                userZombiePackProgressDtos.Add(new UserZombiePackProgressDto
                {
                    Id = _userZombiePackProgressId2,
                    IsDestroyed = true,
                    LastHuntDate = null,
                    LastRegen = DateTime.Today.AddMinutes(-1),
                    MaxZombies = 100,
                    RegenMinuteTicks = 10,
                    RegenZombieRate = 5,
                    UserId = _userId,
                    ZombiePackId = _hotZoneId2,
                    ZombiesLeft = 0
                });

                dataContext.UserZombiePackProgressDtos.InsertAllOnSubmit(userZombiePackProgressDtos);
                dataContext.SubmitChanges();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                dataContext.UserDtos.DeleteAllOnSubmit(
                                dataContext.UserDtos.Where(c => c.Id == _userId));

                dataContext.ZombiePackDtos.DeleteAllOnSubmit(
                                dataContext.ZombiePackDtos.Where(c => c.Id == _zombiePackId1
                                                                    || c.Id == _zombiePackId2));

                dataContext.HotZoneDtos.DeleteAllOnSubmit(
                                dataContext.HotZoneDtos.Where(c => c.Id == _hotZoneId
                                                                    || c.Id == _hotZoneId2));

                dataContext.UserZombiePackProgressDtos.DeleteAllOnSubmit(
                                dataContext.UserZombiePackProgressDtos.Where(c => c.Id == _userZombiePackProgressId
                                                                    || c.Id == _userZombiePackProgressId2));

                dataContext.SubmitChanges();
            }
        }

        [TestMethod]
        public void should_retrieve_user_zombie_pack_progress_for_user()
        {
            IUserZombiePackProgress userZombiePackProgress = _userZombiePackProgressRetriever.GetUserZombiePackProgressFor(_userId, _hotZoneId);

            Assert.AreEqual(false, userZombiePackProgress.IsDestroyed);
            Assert.AreEqual(100, userZombiePackProgress.MaxZombies);
            Assert.AreEqual(100, userZombiePackProgress.ZombiesLeft);
        }

        [TestMethod]
        public void should_return_true_if_zombie_pack_is_destroyed_by_user()
        {
            bool isDestroyed = _userZombiePackProgressRetriever.IsZombiePackDestroyed(_userId, _hotZoneId2);

            Assert.IsTrue(isDestroyed);
        }

        [TestMethod]
        public void should_return_false_if_zombie_pack_is_not_destroyed_by_user()
        {
            bool isDestroyed = _userZombiePackProgressRetriever.IsZombiePackDestroyed(_userId, _hotZoneId);

            Assert.IsFalse(isDestroyed);
        }

        [TestMethod]
        public void should_return_falise_if_user_zombie_pack_progress_does_not_exist()
        {
            bool isDestroyed = _userZombiePackProgressRetriever.IsZombiePackDestroyed(Guid.NewGuid(), Guid.NewGuid());
            Assert.IsFalse(isDestroyed);
        }
    }
}
