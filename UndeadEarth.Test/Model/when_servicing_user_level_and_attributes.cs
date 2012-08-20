using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Model;
using Moq;

namespace UndeadEarth.Test.Model
{
    [TestClass]
    public class when_servicing_user_level_and_attributes
    {

        private Mock<IUserRetriever> _userRetriever;
        private Mock<IUserStatsRetriever> _userStatsRetriever;
        private Mock<IUserSaver> _userSaver; 
        private IUserLevelService _userlevelService;
        private Guid _userId;

        public when_servicing_user_level_and_attributes()
        {
            
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _userRetriever = new Mock<IUserRetriever>();
            _userStatsRetriever = new Mock<IUserStatsRetriever>();
            _userSaver = new Mock<IUserSaver>();
            _userId = Guid.NewGuid();
            _userlevelService = new UserLevelService(_userRetriever.Object, _userStatsRetriever.Object, _userSaver.Object);
        }

        [TestMethod]
        public void when_checking_user_level_and_zombie_kills_to_update_attributes_should_return_if_user_does_not_exist()
        {
            Guid userId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(false);

            _userlevelService.CheckForLevelUp(userId);

            _userStatsRetriever.Verify(c => c.GetStats(It.IsAny<Guid>()), Times.Never());
            _userRetriever.Verify(c => c.GetCurrentLevel(It.IsAny<Guid>()), Times.Never());
            _userSaver.Verify(c => c.SetUserLevel(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
            _userSaver.Verify(c => c.UpdateEnergyForDifficultyCalculation(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
            _userSaver.Verify(c => c.UpdateAttackForDifficultyCalculation(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
            _userSaver.Verify(c => c.UpdateUserInventorySlot(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
        }

        [TestMethod]
        public void when_checking_user_level_and_zombie_kills_to_update_attributes_should_not_update_if_not_enough_zombie_kills()
        {
            Guid userId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
        }

        [TestMethod]
        public void should_return_the_number_of_zombie_kills_needed_for_next_level()
        {
            TheZombieCountForLevel(1).ShouldBe((long)20);
            TheZombieCountForLevel(2).ShouldBe((long)155);
            TheZombieCountForLevel(3).ShouldBe((long)512);
            TheZombieCountForLevel(4).ShouldBe((long)1197);
            TheZombieCountForLevel(9).ShouldBe((long)13107);
            TheZombieCountForLevel(39).ShouldBe((long)993411);
            TheZombieCountForLevel(99).ShouldBe((long)15532346);
        }

        private object TheZombieCountForLevel(int level)
        {
            return _userlevelService.GetZombieCountForLevelUp(level);
        }

        [TestMethod]
        public void should_return_correct_base_attack_power_for_given_level()
        {
            TheAttackPowerForLevel(2).ShouldBe(7);
            TheAttackPowerForLevel(3).ShouldBe(10);
            TheAttackPowerForLevel(4).ShouldBe(13);
            TheAttackPowerForLevel(5).ShouldBe(15);
            TheAttackPowerForLevel(10).ShouldBe(22);
            TheAttackPowerForLevel(40).ShouldBe(35);
            TheAttackPowerForLevel(100).ShouldBe(44);

        }

        private object TheAttackPowerForLevel(int level)
        {
            return _userlevelService.GetBaseAttackPowerAttributeForUserGivenLevel(level);
        }

        [TestMethod]
        public void should_return_correct_energy_for_given_level()
        {
            TheEnergyForLevel(2).ShouldBe(122);
            TheEnergyForLevel(3).ShouldBe(133);
            TheEnergyForLevel(4).ShouldBe(143);
            TheEnergyForLevel(5).ShouldBe(154);
            TheEnergyForLevel(10).ShouldBe(209);
            TheEnergyForLevel(40).ShouldBe(534);
            TheEnergyForLevel(100).ShouldBe(1186);
        }

        private object TheEnergyForLevel(int level)
        {
            return _userlevelService.GetBaseEnergyAttributeForUserGivenLevel(level);
        }

        [TestMethod]
        public void should_return_correct_inventory_slot_number_for_given_level()
        {
            TheItemCountAmountForLevel(2).ShouldBe(6);
            TheItemCountAmountForLevel(3).ShouldBe(7);
            TheItemCountAmountForLevel(4).ShouldBe(7);
            TheItemCountAmountForLevel(5).ShouldBe(7);
            TheItemCountAmountForLevel(10).ShouldBe(8);
            TheItemCountAmountForLevel(40).ShouldBe(12);
            TheItemCountAmountForLevel(100).ShouldBe(15);
        }

        private object TheItemCountAmountForLevel(int level)
        {
            return _userlevelService.GetBaseItemSlotAttributeForUserGivenLevel(level);
        }

        [TestMethod]
        public void should_return_current_level_for_user()
        {
            Guid userId = Guid.NewGuid();
            int userLevel = 5;

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
            _userRetriever.Setup(c => c.GetCurrentLevel(userId)).Returns(userLevel).Verifiable();

            _userlevelService.GetCurrentLevelForUser(userId);

            _userRetriever.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void should_throw_error_if_user_does_not_exist()
        {
            Guid userId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(false);

            _userlevelService.GetCurrentLevelForUser(userId);
        }

        [TestMethod]
        public void should_return_if_user_has_not_killed_enough_zombies_for_next_level()
        {
            Guid userId = Guid.NewGuid();
            long zombiesKilled = 50;
            int userLevel = 2;

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
            _userStatsRetriever.Setup(c => c.GetStats(userId)).Returns(new UserStats { ZombiesKilled = zombiesKilled });
            _userRetriever.Setup(c => c.GetCurrentLevel(userId)).Returns(userLevel);

            _userlevelService.CheckForLevelUp(userId);

            _userSaver.Verify(c => c.SetUserLevel(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
            _userSaver.Verify(c => c.UpdateAttackForDifficultyCalculation(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
            _userSaver.Verify(c => c.UpdateEnergyForDifficultyCalculation(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
            _userSaver.Verify(c => c.UpdateUserInventorySlot(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
        }

        [TestMethod]
        public void should_update_user_level_and_user_base_attack_and_user_base_energy_and_user_inventory_when_user_kills_enough_zombies_to_pass_level()
        {
            Guid userId = Guid.NewGuid();
            long zombiesKilled = 160;
            int userLevel = 2;

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true).Verifiable();
            _userStatsRetriever.Setup(c => c.GetStats(userId)).Returns(new UserStats { ZombiesKilled = zombiesKilled });
            _userRetriever.Setup(c => c.GetCurrentLevel(userId)).Returns(userLevel);

            _userlevelService.CheckForLevelUp(userId);

            _userSaver.Verify(c => c.SetUserLevel(userId, 3), Times.Once());
            _userSaver.Verify(c => c.UpdateCurrentBaseAttack(userId, 10), Times.Once());
            _userSaver.Verify(c => c.UpdateCurrentBaseEnergy(userId, 133), Times.Once());
            _userSaver.Verify(c => c.UpdateUserInventorySlot(userId, 7), Times.Once());
        }

        [TestMethod]
        public void should_replenish_energy_after_level_up()
        {
            GivenTheUserExists(withLevelOf: 2);
            GivenZombiesKilled(160);
            CheckForLevelUp();
            ShouldHaveReplenishedEnergy(toValue: 133);
        }

        private void GivenTheUserExists(int withLevelOf)
        {
            _userRetriever.Setup(c => c.UserExists(_userId)).Returns(true).Verifiable();
            _userRetriever.Setup(c => c.GetCurrentLevel(_userId)).Returns(withLevelOf);
        }

        private void GivenZombiesKilled(int count)
        {
            _userStatsRetriever.Setup(c => c.GetStats(_userId)).Returns(new UserStats { ZombiesKilled = count });
        }

        private void CheckForLevelUp()
        {
            _userlevelService.CheckForLevelUp(_userId);
        }

        private void ShouldHaveReplenishedEnergy(int toValue)
        {
            _userSaver.Verify(s => s.SaveLastEnergy(_userId, toValue, It.IsAny<DateTime>()));
        }
    }

    public static class TestExtensions
    {
        public static void ShouldBe(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
        }
    }
}
