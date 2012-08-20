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
    public class when_hunting_for_zombies
    {
        private IHuntDirector _huntDirector;
        private Guid _userId;
        private Guid _zombiePackId;
        private IUserZombiePackProgress _savedZombiePackProgress;
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IZombiePackRetriever> _zombiePackRetriever;
        private Mock<IUserEnergyProvider> _userEnergyProvider;
        private Mock<IUserSaver> _userSaver;
        private Mock<IUserZombiePackProgressSaver> _userZombiePackProgressSaver;
        private Mock<IUserAttackPowerProvider> _userAttackPowerProvider;
        private Mock<IZombiePackDifficultyDirector> _zombiePackDifficultyDirector;
        private Mock<IRandomNumberProvider> _randomNumberProvider;
        private Mock<IUserLevelService> _userLevelService;
        private Mock<IUserCountsSaver> _userCountsSaver;
        private Mock<IHotZoneRetriever> _hotZoneRetriever;

        public when_hunting_for_zombies()
        {
            
        }

        [TestInitialize]
        public void Initialize()
        {
            _userRetriever = new Mock<IUserRetriever>();
            _zombiePackRetriever = new Mock<IZombiePackRetriever>();
            _userEnergyProvider = new Mock<IUserEnergyProvider>();
            _userSaver = new Mock<IUserSaver>();
            _userZombiePackProgressSaver = new Mock<IUserZombiePackProgressSaver>();
            _userAttackPowerProvider = new Mock<IUserAttackPowerProvider>();
            _zombiePackDifficultyDirector = new Mock<IZombiePackDifficultyDirector>();
            _randomNumberProvider = new Mock<IRandomNumberProvider>();
            _userLevelService = new Mock<IUserLevelService>();
            _userCountsSaver = new Mock<IUserCountsSaver>();
            _hotZoneRetriever = new Mock<IHotZoneRetriever>();
            _userId = Guid.NewGuid();
            _zombiePackId = Guid.NewGuid();
            _huntDirector = new HuntDirector(_userRetriever.Object,
                _zombiePackRetriever.Object,
                _userEnergyProvider.Object,
                _userSaver.Object,
                _userZombiePackProgressSaver.Object,
                _userAttackPowerProvider.Object,
                _zombiePackDifficultyDirector.Object,
                _randomNumberProvider.Object,
                _userLevelService.Object,
                _userCountsSaver.Object,
                _hotZoneRetriever.Object);
        }

        [TestMethod]
        public void should_not_hunt_if_user_doesnt_exist()
        {
            _userRetriever.Setup(s => s.UserExists(_userId)).Returns(false).Verifiable();

            _huntDirector.Hunt(_userId, Guid.NewGuid());

            _userRetriever.Verify();

            ShouldNotHaveHunted();
        }

        [TestMethod]
        public void should_not_hunt_if_zombie_pack_doesnt_exist()
        {
            GivenUserThatExists();
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(_zombiePackId)).Returns(false).Verifiable();

            _huntDirector.Hunt(_userId, _zombiePackId);
            _userRetriever.Verify();
            _zombiePackRetriever.Verify();

            ShouldNotHaveHunted();
        }

        [TestMethod]
        public void should_not_hunt_if_user_is_not_at_zombie_pack_latitude()
        {
            Mock<IUser> user = new Mock<IUser>();
            user.SetupGet(s => s.Latitude).Returns(1);
            user.SetupGet(s => s.Longitude).Returns(2);

            Mock<IZombiePack> zombiePack = new Mock<IZombiePack>();
            zombiePack.SetupGet(s => s.Latitude).Returns(5);
            zombiePack.SetupGet(s => s.Longitude).Returns(2);

            GivenUserThatExists();
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(_zombiePackId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.GetZombiePackById(_zombiePackId)).Returns(zombiePack.Object);
            _userRetriever.Setup(s => s.GetUserById(_userId)).Returns(user.Object);

            _huntDirector.Hunt(_userId, _zombiePackId);

            _userRetriever.Verify();

            ShouldNotHaveHunted();
        }

        [TestMethod]
        public void should_not_hunt_if_user_is_not_at_zombie_pack_longitude()
        {
            Mock<IUser> user = new Mock<IUser>();
            user.SetupGet(s => s.Latitude).Returns(1);
            user.SetupGet(s => s.Longitude).Returns(2);

            Mock<IZombiePack> zombiePack = new Mock<IZombiePack>();
            zombiePack.SetupGet(s => s.Latitude).Returns(1);
            zombiePack.SetupGet(s => s.Longitude).Returns(3);

            GivenUserThatExists();
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(_zombiePackId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.GetZombiePackById(_zombiePackId)).Returns(zombiePack.Object);
            _userRetriever.Setup(s => s.GetUserById(_userId)).Returns(user.Object);

            PerformHunt();
            ShouldNotHaveHunted();
        }

        [TestMethod]
        public void should_not_hunt_if_user_doesnt_have_enough_energy()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenUserEnergy(1);
            GivenEnergyCostToHunt(100);

            PerformHunt();

            //ensure zombie pack difficult was queried for
            _zombiePackDifficultyDirector.Verify();

            //ensure that energy was queried for
            _userEnergyProvider.Verify();

            //verify that hunt was not performed
            ShouldNotHaveHunted();
        }

        [TestMethod]
        public void should_hunt_if_users_has_enough_energy()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenUserEnergy(100);
            GivenEnergyCostToHunt(10);
            GivenAttackPower(1);
            GivenProgress(new UserZombiePackProgress { IsDestroyed = false, MaxZombies = 5, ZombiesLeft = 5 });

            PerformHunt();

            ShouldHaveHunted();
        }

        [TestMethod]
        public void should_decrement_the_zombies_left_in_a_zombie_pack_after_a_hunt()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenUserEnergy(100);
            GivenEnergyCostToHunt(10);
            GivenAttackPower(1);
            GivenProgress(new UserZombiePackProgress { IsDestroyed = false, MaxZombies = 5, ZombiesLeft = 5 });

            PerformHunt();
            ShouldHaveHunted();
            ProgressShouldBe(new UserZombiePackProgress { IsDestroyed = false, MaxZombies = 5, ZombiesLeft = 4 });
        }

        [TestMethod]
        public void should_mark_zombie_pack_as_destroyed_if_the_hunt_destroyed_the_last_zombie()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenUserEnergy(100);
            GivenEnergyCostToHunt(10);
            GivenAttackPower(5);
            GivenProgress(new UserZombiePackProgress { IsDestroyed = false, MaxZombies = 5, ZombiesLeft = 5 });

            PerformHunt();
            ShouldHaveHunted();
            ProgressShouldBe(new UserZombiePackProgress { IsDestroyed = true, MaxZombies = 5, ZombiesLeft = 0 });
        }

        [TestMethod]
        public void should_update_users_energy_if_user_can_hunt()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenUserEnergy(100);
            GivenEnergyCostToHunt(10);
            GivenAttackPower(5);
            GivenProgress(new UserZombiePackProgress { IsDestroyed = false, MaxZombies = 5, ZombiesLeft = 5 });

            PerformHunt();
            NewEnergyShouldBe(90);
        }

        [TestMethod]
        public void should_update_users_zone_id_if_user_is_at_a_zombie_pack_location()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenUserEnergy(100);
            GivenEnergyCostToHunt(10);
            GivenAttackPower(5);
            GivenProgress(new UserZombiePackProgress { IsDestroyed = false, MaxZombies = 5, ZombiesLeft = 5 });
            
            Guid hotZoneId = Guid.NewGuid();
            GivenZombiePackIsAssociatedWithThisHotzone(hotZoneId);

            PerformHunt();
            UserZoneShouldBe(hotZoneId);
        }

        [TestMethod]
        public void should_give_user_a_random_amount_of_money_after_hunt()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenUserEnergy(100);
            GivenEnergyCostToHunt(10);
            GivenAttackPower(5);
            GivenProgress(new UserZombiePackProgress { IsDestroyed = false, MaxZombies = 5, ZombiesLeft = 5 });
            GivenRandomGeneratedNumberForMoneyCalulationOf(7);
            
            PerformHunt();
            ShouldHaveGainedMoneyOf(Convert.ToInt32(5));
        }

        [TestMethod]
        public void should_disregard_hunt_if_zombie_pack_does_not_have_any_zombies()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenUserEnergy(100);
            GivenEnergyCostToHunt(10);
            GivenAttackPower(5);
            GivenProgress(new UserZombiePackProgress { IsDestroyed = false, MaxZombies = 5, ZombiesLeft = 0 });

            PerformHunt();
            ShouldNotHaveHunted();
        }

        [TestMethod]
        public void should_decrement_number_of_zombies_by_users_attack_power_on_successful_hunt()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenUserEnergy(100);
            GivenEnergyCostToHunt(10);
            GivenAttackPower(15);
            GivenProgress(new UserZombiePackProgress { IsDestroyed = false, MaxZombies = 20, ZombiesLeft = 20 });

            PerformHunt();
            ShouldHaveHunted();
            ProgressShouldBe(new UserZombiePackProgress { IsDestroyed = false, MaxZombies = 20, ZombiesLeft = 5 });
        }

        [TestMethod]
        public void should_destroy_zombie_pack_if_users_attack_power_exceeds_the_number_of_zombies()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenUserEnergy(100);
            GivenEnergyCostToHunt(10);
            GivenAttackPower(150);
            GivenProgress(new UserZombiePackProgress { IsDestroyed = false, MaxZombies = 20, ZombiesLeft = 20 });

            PerformHunt();
            ShouldHaveHunted();
            ProgressShouldBe(new UserZombiePackProgress { IsDestroyed = true, MaxZombies = 20, ZombiesLeft = 0 });
        }

        [TestMethod]
        public void should_update_user_last_hotzone_location_after_successful_hunt()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenAttackPower(1);
            GivenEnergyCostToHunt(1);
            GivenUserEnergy(10);
            
            Guid hotZoneId = Guid.NewGuid();
            GivenZombiePackIsAssociatedWithThisHotzone(hotZoneId);
            GivenProgress(new UserZombiePackProgress { MaxZombies = 5, ZombiesLeft = 5 });

            PerformHunt();
            ShouldHaveHunted();
            LastHotZoneShouldBe(hotZoneId);
        }

        [TestMethod]
        public void should_check_users_level_after_hunt()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenAttackPower(1);
            GivenEnergyCostToHunt(1);
            GivenUserEnergy(10);
            GivenProgress(new UserZombiePackProgress { MaxZombies = 5, ZombiesLeft = 5 });

            PerformHunt();

            ShouldCheckUserLevel();
        }

        [TestMethod]
        public void should_record_zombies_killed_after_hunt()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenAttackPower(100);
            GivenEnergyCostToHunt(1);
            GivenUserEnergy(10);
            GivenProgress(new UserZombiePackProgress { MaxZombies = 5, ZombiesLeft = 5 });

            PerformHunt();

            ShouldRecordZombiesKilled(5);
        }

        [TestMethod]
        public void should_record_hotzone_destoryed_if_hunting_cleared_hotzone()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenAttackPower(100);
            GivenEnergyCostToHunt(1);
            GivenUserEnergy(10);
            GivenProgress(new UserZombiePackProgress { MaxZombies = 5, ZombiesLeft = 5 });

            GivenThatZombiePackWasTheLastOne();

            PerformHunt();

            ShouldRecordHotZoneDestroyed();
        }

        [TestMethod]
        public void should_record_peak_zombies_destroyed()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenAttackPower(100);
            GivenEnergyCostToHunt(1);
            GivenUserEnergy(10);
            GivenProgress(new UserZombiePackProgress { MaxZombies = 5, ZombiesLeft = 5 });

            GivenThatZombiePackWasTheLastOne();

            PerformHunt();

            ShouldRecordPeakZombiesDestroyed(5);
        }

        [TestMethod]
        public void should_record_zombiepack_destoryed_if_zombiepack_cleared()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenAttackPower(100);
            GivenEnergyCostToHunt(1);
            GivenUserEnergy(10);
            GivenProgress(new UserZombiePackProgress { MaxZombies = 5, ZombiesLeft = 5 });

            GivenThatZombiePackWasTheLastOne();

            PerformHunt();

            ShouldRecordZombiePackDestroyed();
        }

        [TestMethod]
        public void should_record_money_earned_after_a_succesful_hunt()
        {
            GivenUserThatExists();
            GivenZombiePackExistsAndUserIsAtZombiePack();
            GivenAttackPower(100);
            GivenEnergyCostToHunt(1);
            GivenUserEnergy(10);
            GivenProgress(new UserZombiePackProgress { MaxZombies = 5, ZombiesLeft = 5 });
            GivenRandomGeneratedNumberForMoneyCalulationOf(10);

            PerformHunt();

            ShouldRecordMoneyAccumulated(5);
        }

        private void GivenUserThatExists()
        {
            _userRetriever.Setup(s => s.UserExists(_userId)).Returns(true).Verifiable();
        }

        private void GivenZombiePackExistsAndUserIsAtZombiePack()
        {
            Mock<IUser> user = new Mock<IUser>();
            user.SetupGet(s => s.Latitude).Returns(1);
            user.SetupGet(s => s.Longitude).Returns(2);

            Mock<IZombiePack> zombiePack = new Mock<IZombiePack>();
            zombiePack.SetupGet(s => s.Latitude).Returns(1);
            zombiePack.SetupGet(s => s.Longitude).Returns(2);

            _zombiePackRetriever.Setup(s => s.ZombiePackExists(_zombiePackId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.GetZombiePackById(_zombiePackId)).Returns(zombiePack.Object);
            _userRetriever.Setup(s => s.GetUserById(_userId)).Returns(user.Object);
        }

        private void GivenEnergyCostToHunt(int energyCost)
        {
            _zombiePackDifficultyDirector.Setup(s => s.GetEnergyCost(_userId, _zombiePackId)).Returns(energyCost).Verifiable();
        }

        private void GivenUserEnergy(int userEnergy)
        {
            _userEnergyProvider.Setup(s => s.GetUserEnergy(_userId, It.IsAny<DateTime>())).Returns(userEnergy).Verifiable();
        }

        private void PerformHunt()
        {
            _userZombiePackProgressSaver.Setup(s => s.SaveZombiePackProgress(_userId, _zombiePackId, It.IsAny<IUserZombiePackProgress>()))
                                        .Callback<Guid, Guid, IUserZombiePackProgress>((passedInUserId, passedInZombiePackId, progress) => _savedZombiePackProgress = progress);

            _huntDirector.Hunt(_userId, _zombiePackId);
        }

        private void GivenAttackPower(int attackPower)
        {
            _userAttackPowerProvider.Setup(s => s.GetAttackPower(_userId)).Returns(attackPower).Verifiable();
        }

        private void GivenProgress(UserZombiePackProgress userZombiePackProgress)
        {

            _zombiePackDifficultyDirector.Setup(s => s.GetCalculatedZombiePackProgress(_userId, _zombiePackId)).Returns(userZombiePackProgress);
        }

        private void ShouldHaveHunted()
        {
            _userZombiePackProgressSaver.Verify(
                            s => s.SaveZombiePackProgress(_userId, _zombiePackId, It.IsAny<IUserZombiePackProgress>()));
        }

        private void ShouldNotHaveHunted()
        {
            _userZombiePackProgressSaver.Verify(
                            s => s.SaveZombiePackProgress(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<IUserZombiePackProgress>()),
                            Times.Never());
        }

        private void NewEnergyShouldBe(int energy)
        {
            _userSaver.Verify(s => s.SaveLastEnergy(_userId, energy, It.IsAny<DateTime>()));
        }

        private void ProgressShouldBe(UserZombiePackProgress userZombiePackProgress)
        {
            Assert.AreEqual(userZombiePackProgress.IsDestroyed, _savedZombiePackProgress.IsDestroyed);
            Assert.AreEqual(userZombiePackProgress.MaxZombies, _savedZombiePackProgress.MaxZombies);
            Assert.AreEqual(userZombiePackProgress.ZombiesLeft, _savedZombiePackProgress.ZombiesLeft);
        }

        private void GivenZombiePackIsAssociatedWithThisHotzone(Guid hotZoneId)
        {
            _zombiePackRetriever.Setup(s => s.GetHotZoneByZombiePackId(_zombiePackId)).Returns(hotZoneId);
        }

        private void UserZoneShouldBe(Guid hotZoneId)
        {
            _userSaver.Verify(s => s.UpdateZone(_userId, hotZoneId));
        }

        private void GivenRandomGeneratedNumberForMoneyCalulationOf(int money)
        {
            _randomNumberProvider.Setup(s => s.GetRandomInclusive(It.IsAny<int>(), It.IsAny<int>())).Returns(money);
        }

        private void ShouldHaveGainedMoneyOf(int money)
        {
            _userSaver.Verify(s => s.AddMoney(_userId, money));
        }

        private void LastHotZoneShouldBe(Guid hotZoneId)
        {
            _userSaver.Verify(s => s.UpdateLastVisitedHotZone(_userId, hotZoneId));
        }

        private void ShouldCheckUserLevel()
        {
            _userLevelService.Verify(c => c.CheckForLevelUp(_userId), Times.AtLeastOnce());
        }

        private void ShouldRecordZombiesKilled(int zombiesKilled)
        {
            _userCountsSaver.Verify(s => s.AddZombiesKilled(_userId, zombiesKilled));
        }

        private void GivenThatZombiePackWasTheLastOne()
        {
            _hotZoneRetriever.Setup(s => s.ZombiePacksLeft(_userId, It.IsAny<Guid>())).Returns(0);
        }

        private void ShouldRecordHotZoneDestroyed()
        {
            _userCountsSaver.Verify(s => s.AddHotZonesDestroyed(_userId, 1));
        }

        private void ShouldRecordPeakZombiesDestroyed(int zombiesDestroyed)
        {
            _userCountsSaver.Verify(s => s.RecordPeakZombiesDestroyed(_userId, zombiesDestroyed));
        }

        private void ShouldRecordZombiePackDestroyed()
        {
            _userCountsSaver.Verify(s => s.AddZombiePacksDestroyed(_userId, 1));
        }
        private void ShouldRecordMoneyAccumulated(int money)
        {
            _userCountsSaver.Verify(s => s.AddMoney(_userId, money));
        }
    }
}
