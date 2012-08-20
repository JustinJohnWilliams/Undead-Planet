using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Test.Dal.Utility;
using System.Linq;
using UndeadEarth.Contract;
using UndeadEarth.Dal;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_saving_user_counts
    {
        private string _connectionString;
        private IUserCountsSaver _userCountsSaver = null;
        private Guid _userId;

        /// <summary>
        /// Initializes a new instance of the when_adding_miles class.
        /// </summary>
        public when_saving_user_counts()
        {
            _connectionString = DalTestContextSpecification.ConnectionString;
            _userId = Guid.NewGuid();
            _userCountsSaver = new UserCountsRepository(_connectionString);
        }

        [TestMethod]
        public void should_insert_counts()
        {
            _userCountsSaver.InsertCounts(_userId);

            UserCountDto counts = TheCountsForUser(_userId);
            counts.UserId.ShouldBe(_userId);
            counts.AccumulatedMoney.ShouldBe(0L);
            counts.HotZonesDestroyed.ShouldBe(0L);
            counts.Miles.ShouldBe(0L);
            counts.PeakAttack.ShouldBe(0L);
            counts.ZombiePacksDestroyed.ShouldBe(0L);
            counts.ZombiesKilled.ShouldBe(0L);
        }

        [TestMethod]
        public void should_create_counts_record_if_none_exists_for_user()
        {
            _userCountsSaver.AddMiles(_userId, 10);

            TheCountsForUser(_userId).Miles.ShouldBe(10L);
        }

        private UserCountDto TheCountsForUser(Guid _userId)
        {
            using (TestDataContext testDataContext = new TestDataContext(_connectionString))
            {
                UserCountDto userCountDto = testDataContext.UserCountDtos.Single(s => s.UserId == _userId);
                return userCountDto;
            }
        }

        [TestMethod]
        public void should_increment_miles_count_for_user()
        {
            _userCountsSaver.AddMiles(_userId, 10);
            _userCountsSaver.AddMiles(_userId, 15);

            TheCountsForUser(_userId).Miles.ShouldBe(25L);
        }

        [TestMethod]
        public void should_increment_zombies_killed_for_user()
        {
            _userCountsSaver.AddZombiesKilled(_userId, 10);
            _userCountsSaver.AddZombiesKilled(_userId, 15);

            TheCountsForUser(_userId).ZombiesKilled.ShouldBe(25L);
        }

        [TestMethod]
        public void should_increment_hotzones_destroyed()
        {
            _userCountsSaver.AddHotZonesDestroyed(_userId, 10);
            _userCountsSaver.AddHotZonesDestroyed(_userId, 15);

            TheCountsForUser(_userId).HotZonesDestroyed.Value.ShouldBe(25L);
        }

        [TestMethod]
        public void should_set_peak_attack_if_greater_than_whats_currently_saved()
        {
            _userCountsSaver.RecordPeakZombiesDestroyed(_userId, 10);
            _userCountsSaver.RecordPeakZombiesDestroyed(_userId, 6);

            TheCountsForUser(_userId).PeakAttack.ShouldBe(10L);
        }

        [TestMethod]
        public void should_increment_zombie_pack_destroyed_count()
        {
            _userCountsSaver.AddZombiePacksDestroyed(_userId, 10);
            _userCountsSaver.AddZombiePacksDestroyed(_userId, 15);

            TheCountsForUser(_userId).ZombiePacksDestroyed.ShouldBe(25L);
        }

        [TestMethod]
        public void should_increment_accumulated_money()
        {
            _userCountsSaver.AddMoney(_userId, 10);
            _userCountsSaver.AddMoney(_userId, 15);

            TheCountsForUser(_userId).AccumulatedMoney.ShouldBe(25L);
        }
        
        [TestCleanup]
        public void TestCleanup()
        {
            _connectionString = DalTestContextSpecification.ConnectionString;

            using (TestDataContext testDataContext = new TestDataContext(_connectionString))
            {
                testDataContext.UserCountDtos.DeleteAllOnSubmit(
                        testDataContext.UserCountDtos.Where(s => s.UserId == _userId));

                testDataContext.SubmitChanges();
            }
        }
    }

}
