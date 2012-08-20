using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Dal;
using UndeadEarth.Contract;
using UndeadEarth.Test.Dal.Utility;

namespace UndeadEarth.Test.Dal
{
    /// <summary>
    /// Summary description for when_retrieving_user_counts
    /// </summary>
    [TestClass]
    public class when_retrieving_user_counts
    {
        private string _connectionString;
        private IUserStatsRetriever _userStatsRetriever;
        private Guid _userId;
        private Guid _nonExistantUserId;

        /// <summary>
        /// Initializes a new instance of the when_adding_miles class.
        /// </summary>
        public when_retrieving_user_counts()
        {
            _connectionString = DalTestContextSpecification.ConnectionString;
            _userId = Guid.NewGuid();
            _nonExistantUserId = Guid.NewGuid();
            _userStatsRetriever = new UserCountsRepository(_connectionString);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            TestDataContext testDataContext = new TestDataContext(_connectionString);
            var userCountDto = new UserCountDto
            {
                Id = Guid.NewGuid(),
                HotZonesDestroyed = 10,
                Miles = 11,
                UserId = _userId,
                PeakAttack = 111,
                ZombiesKilled = 1111,
                ZombiePacksDestroyed = 11111,
                AccumulatedMoney = 500
            };

            testDataContext.UserCountDtos.InsertOnSubmit(userCountDto);
            testDataContext.SubmitChanges();

            UserDto userDto = new UserDto { Id = _userId, DisplayName = "name", Email = string.Empty };
            testDataContext.UserDtos.InsertOnSubmit(userDto);
            testDataContext.SubmitChanges();
        }

        [TestMethod]
        public void should_not_insert_empty_record_for_user_if_non_exists()
        {
            long miles = _userStatsRetriever.GetStats(_nonExistantUserId).MilesTraveled;
            Assert.AreEqual(0, miles);

            TestDataContext testDataContext = new TestDataContext(_connectionString);
            Assert.IsFalse(testDataContext.UserCountDtos.Any(s => s.UserId == _nonExistantUserId));
        }

        [TestMethod]
        public void should_retrieve_miles_traveled_for_user_that_exists()
        {
            long miles = _userStatsRetriever.GetStats(_userId).MilesTraveled;
            Assert.AreEqual((long)11, miles);
        }

        [TestMethod]
        public void should_retrieve_zombies_detroyed()
        {
            long zombies = _userStatsRetriever.GetStats(_userId).ZombiesKilled;
            Assert.AreEqual((long)1111, zombies);
        }

        [TestMethod]
        public void should_return_zombie_packs_destroyed()
        {
            long zombies = _userStatsRetriever.GetStats(_userId).ZombiePacksDestroyed;
            Assert.AreEqual((long)11111, zombies);
        }

        [TestMethod]
        public void should_return_hotzones_destroyed()
        {
            long hotzones = _userStatsRetriever.GetStats(_userId).HotZonesDestroyed;
            Assert.AreEqual((long)10, hotzones);
        }

        [TestMethod]
        public void should_return_peak_attack()
        {
            long peakattack = _userStatsRetriever.GetStats(_userId).KillStreak;
            Assert.AreEqual((long)111, peakattack);
        }

        [TestMethod]
        public void should_return_accumulated_money()
        {
            long money = _userStatsRetriever.GetStats(_userId).MoneyAccumulated;
            Assert.AreEqual((long)500, money);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _connectionString = DalTestContextSpecification.ConnectionString;
            TestDataContext testDataContext = new TestDataContext(_connectionString);
            testDataContext.UserCountDtos.DeleteOnSubmit(testDataContext.UserCountDtos.Single(s => s.UserId == _userId));
            if (testDataContext.UserCountDtos.Any(s => s.UserId == _nonExistantUserId))
            {
                testDataContext.UserCountDtos.DeleteOnSubmit(testDataContext.UserCountDtos.Single(s => s.UserId == _nonExistantUserId));
            }

            testDataContext.UserDtos.DeleteAllOnSubmit(testDataContext.UserDtos.Where(u => u.Id == _userId));
            testDataContext.SubmitChanges();
        }
    }
}

