using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using Moq;
using UndeadEarth.Model;

namespace UndeadEarth.Test.Model
{
    [TestClass]
    public class when_retrieving_achievements
    {
        private Mock<IUserStatsRetriever> _userStatsRetriever;
        private IAchievementProvider _achievementProvider;
        private Guid _userId;

        [TestInitialize]
        public void TestInitialize()
        {
            _userStatsRetriever = new Mock<IUserStatsRetriever>();
            _achievementProvider = new AchievementProvider(_userStatsRetriever.Object);
            _userId = Guid.NewGuid();
        }

        [TestMethod]
        public void should_return_achievement_for_traveling_10_miles()
        {
            GivenMilesTraveledForUser(10);
            ShouldContainAchievment("First Steps - Travel 10 miles.");
        }

        [TestMethod]
        public void should_return_achievement_for_traveling_100_miles()
        {
            GivenMilesTraveledForUser(100);
            ShouldContainAchievment("Tourist - Travel 100 miles.");
        }

        [TestMethod]
        public void should_return_achievement_for_traveling_1000_miles()
        {
            GivenMilesTraveledForUser(1000);
            ShouldContainAchievment("Nomadic Hunter - Travel 1,000 miles");
        }

        [TestMethod]
        public void should_return_achievement_for_traveling_10000_miles()
        {
            GivenMilesTraveledForUser(10000);
            ShouldContainAchievment("Around the World - Travel 10,000 miles");
        }

        [TestMethod]
        public void should_return_achievement_for_traveling_100000_miles()
        {
            GivenMilesTraveledForUser(100000);
            ShouldContainAchievment("No Stone Unturned - Travel 100,000 miles");
        }

        [TestMethod]
        public void should_return_achievement_for_destroying_1_zombiePack()
        {
            GivenZombiePacksDestroyed(1);
            ShouldContainAchievment("First Pack - Destroy 1 Zombie Pack.");
        }

        [TestMethod]
        public void should_return_achievement_for_destroying_10_zombie_packs()
        {
            GivenZombiePacksDestroyed(10);
            ShouldContainAchievment("City Block - Destroy 10 Zombie Packs.");
        }

        [TestMethod]
        public void should_return_achievement_for_destroying_100_zombie_packs()
        {
            GivenZombiePacksDestroyed(100);
            ShouldContainAchievment("Rampage - Destroy 100 Zombie Packs.");
        }

        [TestMethod]
        public void should_return_achievement_for_destroying_1000_zombie_packs()
        {
            GivenZombiePacksDestroyed(1000);
            ShouldContainAchievment("Get er Done - Destroy 1,000 Zombie Packs.");
        }

        [TestMethod]
        public void should_return_achievement_for_destroying_10000_zombie_packs()
        {
            GivenZombiePacksDestroyed(10000);
            ShouldContainAchievment("Madness - Destroy 10,000 Zombie Packs.");
        }

        [TestMethod]
        public void should_return_achievement_for_killing_1_zombie()
        {
            GivenZombiesKilled(1);
            ShouldContainAchievment("First Kill - Kill 1 zombie.");
        }

        [TestMethod]
        public void should_return_achievement_for_killing_10_zombies()
        {
            GivenZombiesKilled(10);
            ShouldContainAchievment("First Ten - Kill 10 zombies.");
        }

        [TestMethod]
        public void should_return_achievement_for_killing_100_zombies()
        {
            GivenZombiesKilled(100);
            ShouldContainAchievment("Hunter - Kill 100 zombies.");
        }

        [TestMethod]
        public void should_return_achievement_for_killing_1000_zombies()
        {
            GivenZombiesKilled(1000);
            ShouldContainAchievment("Genghis Khan - Kill 1,000 zombies.");
        }

        [TestMethod]
        public void should_return_achievement_for_killing_10000_zombies()
        {
            GivenZombiesKilled(10000);
            ShouldContainAchievment("Clearing House - Kill 10,000 zombies.");
        }

        [TestMethod]
        public void should_return_achievement_for_killing_100000_zombies()
        {
            GivenZombiesKilled(100000);
            ShouldContainAchievment("Killtacular - Kill 100,000 zombies.");
        }

        [TestMethod]
        public void should_return_achievement_for_killing_1000000_zombies()
        {
            GivenZombiesKilled(1000000);
            ShouldContainAchievment("Genocide - Kill 1,000,000 zombies.");
        }

        [TestMethod]
        public void should_return_achievement_for_destroying_hotzone()
        {
            GivenHotZonesDestroyed(1);
            ShouldContainAchievment("Apprentice Hunter - Destroy 1 Hot Zone.");
        }

        [TestMethod]
        public void should_return_achivement_for_destroying_10_hotzones()
        {
            GivenHotZonesDestroyed(10);
            ShouldContainAchievment("Journey Man - Destroy 10 Hot Zones.");
        }

        [TestMethod]
        public void should_return_achievement_for_destroying_100_hotzones()
        {
            GivenHotZonesDestroyed(100);
            ShouldContainAchievment("Master Hunter - Destroy 100 Hot Zones.");
        }

        [TestMethod]
        public void should_return_achievement_for_peak_zombies_of_two()
        {
            GivenPeakAttackPower(2);
            ShouldContainAchievment("Double Kill - Kill 2 zombies in one hunt.");
        }

        [TestMethod]
        public void should_return_achievement_for_peak_zombies_10()
        {
            GivenPeakAttackPower(10);
            ShouldContainAchievment("Multi Kill - Kill 10 zombies in one hunt.");
        }

        [TestMethod]
        public void should_return_achievement_for_peak_zombies_100()
        {
            GivenPeakAttackPower(100);
            ShouldContainAchievment("Power Overwhelming - Kill 100 zombies in one hunt.");
        }

        [TestMethod]
        public void should_return_achievement_for_peak_zombies_1000()
        {
            GivenPeakAttackPower(1000);
            ShouldContainAchievment("God Like - Kill 1000 zombies in one hunt.");
        }

        [TestMethod]
        public void should_return_achievement_for_collecting_10_dollars()
        {
            GivenAccumulatedMoney(10);
            ShouldContainAchievment("Payday - Accumulate $10.");
        }

        [TestMethod]
        public void should_return_achievement_for_collecting_100_dollars()
        {
            GivenAccumulatedMoney(100);
            ShouldContainAchievment("Benjamin - Accumulate $100.");
        }

        [TestMethod]
        public void should_return_achievement_for_collecting_1000_dollars()
        {
            GivenAccumulatedMoney(1000);
            ShouldContainAchievment("G's - Accumulate $1,000.");
        }

        [TestMethod]
        public void should_return_achievement_for_collecting_10000_dollars()
        {
            GivenAccumulatedMoney(10000);
            ShouldContainAchievment("Movin on up - Accumulate $10,000.");
        }

        [TestMethod]
        public void should_return_achievement_for_collecting_100000_dollars()
        {
            GivenAccumulatedMoney(100000);
            ShouldContainAchievment("Six Figs - Accumulate $100,000.");
        }

        [TestMethod]
        public void should_return_achievement_for_collecting_1000000_dollars()
        {
            GivenAccumulatedMoney(1000000);
            ShouldContainAchievment("Millionaire - Accumulate $1,000,000.");
        }

        private void GivenMilesTraveledForUser(int miles)
        {
            UserStats userStats = new UserStats { MilesTraveled = miles };
            _userStatsRetriever.Setup(s => s.GetStats(_userId)).Returns(userStats);
        }

        private void GivenZombiePacksDestroyed(int zombiePacks)
        {
            UserStats userStats = new UserStats { ZombiePacksDestroyed = zombiePacks };
            _userStatsRetriever.Setup(s => s.GetStats(_userId)).Returns(userStats);
        }

        private void GivenZombiesKilled(int zombies)
        {
            UserStats userStats = new UserStats { ZombiesKilled = zombies };
            _userStatsRetriever.Setup(s => s.GetStats(_userId)).Returns(userStats);
        }

        private void GivenHotZonesDestroyed(int hotzones)
        {
            UserStats userStats = new UserStats { HotZonesDestroyed = hotzones };
            _userStatsRetriever.Setup(s => s.GetStats(_userId)).Returns(userStats);
        }

        private void GivenPeakAttackPower(int attackPower)
        {
            UserStats userStats = new UserStats { KillStreak = attackPower };
            _userStatsRetriever.Setup(s => s.GetStats(_userId)).Returns(userStats);
        }

        private void ShouldContainAchievment(string message)
        {
            List<string> achievements = _achievementProvider.GetAchievementsForUser(_userId);
            Assert.IsTrue(achievements.Contains(message));
        }
        
        private void GivenAccumulatedMoney(int money)
        {
            UserStats userStats = new UserStats { MoneyAccumulated = money };
            _userStatsRetriever.Setup(s => s.GetStats(_userId)).Returns(userStats);
        }
    }
}
