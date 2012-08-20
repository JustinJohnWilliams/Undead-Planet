using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Model;
using Moq;

namespace UndeadEarth.Test.Model
{
    [TestClass]
    public class when_determining_zombie_pack_progress
    {
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IZombiePackRetriever> _zombiePackRetriever;
        private Mock<IUserZombiePackProgressRetriever> _userZombiePackProgressRetriever;
        private Mock<IRandomNumberProvider> _randomNumberProvider;
        private Mock<IUserSaver> _userSaver;
        private Mock<IUserPotentialProvider> _userPotentialProvider;
        private Mock<IUserZombiePackProgressSaver> _userZombiePackProgressSaver;
        private IZombiePackDifficultyDirector _zombiePackDifficultyDirector;
        private Mock<IUserStatsRetriever> _userStatsRetriever;

        public when_determining_zombie_pack_progress()
        {
            _userRetriever = new Mock<IUserRetriever>();
            _zombiePackRetriever = new Mock<IZombiePackRetriever>();
            _userZombiePackProgressRetriever = new Mock<IUserZombiePackProgressRetriever>();
            _randomNumberProvider = new Mock<IRandomNumberProvider>();
            _userSaver = new Mock<IUserSaver>();
            _userPotentialProvider = new Mock<IUserPotentialProvider>();
            _userZombiePackProgressSaver = new Mock<IUserZombiePackProgressSaver>();
            _userStatsRetriever = new Mock<IUserStatsRetriever>();
            _zombiePackDifficultyDirector = new ZombiePackDifficultyDirector(_userRetriever.Object, 
                                                _zombiePackRetriever.Object, 
                                                _userZombiePackProgressRetriever.Object, 
                                                _userSaver.Object, 
                                                _randomNumberProvider.Object, 
                                                _userPotentialProvider.Object, 
                                                _userZombiePackProgressSaver.Object,
                                                _userStatsRetriever.Object);
        }

        [TestMethod]
        public void should_return_an_empty_progress_if_user_does_not_exist()
        {
            Guid userId = Guid.NewGuid();
            Guid zombiePackId = Guid.NewGuid();

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(false).Verifiable();
            IUserZombiePackProgress progress = _zombiePackDifficultyDirector.GetCalculatedZombiePackProgress(userId, zombiePackId);
            Assert.AreEqual(0, progress.MaxZombies);
            Assert.AreEqual(0, progress.ZombiesLeft);
            Assert.AreEqual(false, progress.IsDestroyed);
            _userRetriever.Verify();
        }

        [TestMethod]
        public void should_return_an_empty_progress_if_zombie_pack_doesnt_exist()
        {
            Guid userId = Guid.NewGuid();
            Guid zombiePackId = Guid.NewGuid();

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(zombiePackId)).Returns(false).Verifiable();
            IUserZombiePackProgress progress = _zombiePackDifficultyDirector.GetCalculatedZombiePackProgress(userId, zombiePackId);
            Assert.AreEqual(0, progress.MaxZombies);
            Assert.AreEqual(0, progress.ZombiesLeft);
            Assert.AreEqual(false, progress.IsDestroyed);
            _zombiePackRetriever.Verify();
        }

        [TestMethod]
        public void should_return_current_zombie_pack_progress_if_user_has_already_visited_zombie_pack()
        {
            Guid userId = Guid.NewGuid();
            Guid zombiePackId = Guid.NewGuid();

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(zombiePackId)).Returns(true);

            UserZombiePackProgress progress = new UserZombiePackProgress{ IsDestroyed = false, MaxZombies = 10, ZombiesLeft = 5 };
            _userZombiePackProgressRetriever.Setup(s => s.GetUserZombiePackProgressFor(userId, zombiePackId))
                                            .Returns(progress as IUserZombiePackProgress)
                                            .Verifiable();

            IUserZombiePackProgress returnedProgress = _zombiePackDifficultyDirector.GetCalculatedZombiePackProgress(userId, zombiePackId);
            Assert.AreEqual(false, returnedProgress.IsDestroyed);
            Assert.AreEqual(10, returnedProgress.MaxZombies);
            Assert.AreEqual(5, returnedProgress.ZombiesLeft);
            _userZombiePackProgressRetriever.Verify();
        }

        [TestMethod]
        public void should_set_base_line_for_users_max_potential_if_last_hotzone_visited_does_not_equal_the_hotzone_for_the_current_zombie_pack_for_every_third_hotzone()
        {
            Guid userId = Guid.NewGuid();
            Guid zombiePackId = Guid.NewGuid();
            Guid lastVisitedHotZoneId = Guid.NewGuid();
            Guid hotzoneIdAssociatedWithZombiePack = Guid.NewGuid();
            int maxPotentialUserAttackPower = 10;
            int maxPotentialUserEnergy = 100;

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(zombiePackId)).Returns(true);

            _userZombiePackProgressRetriever.Setup(s => s.GetUserZombiePackProgressFor(userId, zombiePackId))
                                            .Returns(() => null);

            _userStatsRetriever.Setup(s => s.GetStats(userId)).Returns(new UserStats { HotZonesDestroyed = 3 });

            _userRetriever.Setup(s => s.GetLastVisitedHotZone(userId)).Returns(lastVisitedHotZoneId).Verifiable();
            _zombiePackRetriever.Setup(s => s.GetHotZoneByZombiePackId(zombiePackId)).Returns(hotzoneIdAssociatedWithZombiePack).Verifiable();
            _userPotentialProvider.Setup(s => s.GetMaxPotentialAttackPower(userId)).Returns(maxPotentialUserAttackPower).Verifiable();
            _userPotentialProvider.Setup(s => s.GetMaxPotentialEnergy(userId)).Returns(maxPotentialUserEnergy).Verifiable();

            _zombiePackDifficultyDirector.GetCalculatedZombiePackProgress(userId, zombiePackId);
            _userRetriever.Verify();
            _zombiePackRetriever.Verify();
            _userPotentialProvider.Verify();
            _userPotentialProvider.Verify();
            _userSaver.Verify(s => s.UpdateBaseLine(userId, maxPotentialUserAttackPower, maxPotentialUserEnergy));
        }

        [TestMethod]
        public void should_set_base_line_for_users_max_potential_if_last_hotzone_visited_is_null()
        {
            Guid userId = Guid.NewGuid();
            Guid zombiePackId = Guid.NewGuid();
            Guid? lastVisitedHotZoneId = null;
            Guid hotzoneIdAssociatedWithZombiePack = Guid.NewGuid();
            int maxPotentialUserAttackPower = 10;
            int maxPotentialUserEnergy = 100;

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(zombiePackId)).Returns(true);

            _userZombiePackProgressRetriever.Setup(s => s.GetUserZombiePackProgressFor(userId, zombiePackId))
                                            .Returns(() => null);

            _userStatsRetriever.Setup(s => s.GetStats(userId)).Returns(new UserStats { HotZonesDestroyed = 0 });

            _userRetriever.Setup(s => s.GetLastVisitedHotZone(userId)).Returns(lastVisitedHotZoneId).Verifiable();
            _zombiePackRetriever.Setup(s => s.GetHotZoneByZombiePackId(zombiePackId)).Returns(hotzoneIdAssociatedWithZombiePack).Verifiable();
            _userPotentialProvider.Setup(s => s.GetMaxPotentialAttackPower(userId)).Returns(maxPotentialUserAttackPower).Verifiable();
            _userPotentialProvider.Setup(s => s.GetMaxPotentialEnergy(userId)).Returns(maxPotentialUserEnergy).Verifiable();

            _zombiePackDifficultyDirector.GetCalculatedZombiePackProgress(userId, zombiePackId);
            _userRetriever.Verify();
            _zombiePackRetriever.Verify();
            _userPotentialProvider.Verify();
            _userPotentialProvider.Verify();
            _userSaver.Verify(s => s.UpdateBaseLine(userId, maxPotentialUserAttackPower, maxPotentialUserEnergy));
        }

        [TestMethod]
        public void should_save_and_return_zombie_pack_progress_with_random_difficulty_if_its_the_first_time_user_has_visited_zombie_pack()
        {
            Guid userId = Guid.NewGuid();
            Guid zombiePackId = Guid.NewGuid();
            Guid lastVisitedHotZoneId = Guid.NewGuid();
            Guid hotzoneIdAssociatedWithZombiePack = lastVisitedHotZoneId;
            int baseLineAttackPower = 10;
            int randomNumber = 5;
            IUserZombiePackProgress progressThatWasSaved = null;

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(zombiePackId)).Returns(true);

            _userZombiePackProgressRetriever.Setup(s => s.GetUserZombiePackProgressFor(userId, zombiePackId))
                                            .Returns(() => null);

            _userRetriever.Setup(s => s.GetLastVisitedHotZone(userId)).Returns(lastVisitedHotZoneId);
            _zombiePackRetriever.Setup(s => s.GetHotZoneByZombiePackId(zombiePackId)).Returns(hotzoneIdAssociatedWithZombiePack);
            _userRetriever.Setup(s => s.GetAttackPowerForDifficultyCalculation(userId)).Returns(baseLineAttackPower).Verifiable();
            _randomNumberProvider.Setup(s => s.GetRandomInclusive(It.IsAny<int>(), It.IsAny<int>())).Returns(randomNumber);
            _userZombiePackProgressSaver.Setup(s => s.SaveZombiePackProgress(userId, zombiePackId, It.IsAny<IUserZombiePackProgress>()))
                                        .Callback<Guid, Guid, IUserZombiePackProgress>((callbackUserId, callbackZombiePackId, callbackProgress) => progressThatWasSaved = callbackProgress)
                                        .Verifiable();

            IUserZombiePackProgress progress = _zombiePackDifficultyDirector.GetCalculatedZombiePackProgress(userId, zombiePackId);
            Assert.AreEqual(false, progress.IsDestroyed);
            Assert.AreEqual(50, progress.MaxZombies);
            Assert.AreEqual(50, progress.ZombiesLeft);

            Assert.AreEqual(false, progressThatWasSaved.IsDestroyed);
            Assert.AreEqual(50, progressThatWasSaved.MaxZombies);
            Assert.AreEqual(50, progressThatWasSaved.ZombiesLeft);

            _userZombiePackProgressSaver.Verify();
            _userRetriever.Verify();
        }

        [TestMethod]
        public void should_return_a_hunt_count_of_1_to_4_if_resolved_as_an_easy_hotzone()
        {
            Guid userId = Guid.NewGuid();
            Guid zombiePackId = Guid.NewGuid();
            Guid lastVisitedHotZoneId = Guid.NewGuid();
            Guid hotzoneIdAssociatedWithZombiePack = lastVisitedHotZoneId;
            int baseLineAttackPower = 10;
            int difficulty = 1;

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(zombiePackId)).Returns(true);

            _userZombiePackProgressRetriever.Setup(s => s.GetUserZombiePackProgressFor(userId, zombiePackId))
                                            .Returns(() => null);

            _userRetriever.Setup(s => s.GetLastVisitedHotZone(userId)).Returns(lastVisitedHotZoneId);
            _zombiePackRetriever.Setup(s => s.GetHotZoneByZombiePackId(zombiePackId)).Returns(hotzoneIdAssociatedWithZombiePack);
            _userRetriever.Setup(s => s.GetAttackPowerForDifficultyCalculation(userId)).Returns(baseLineAttackPower).Verifiable();
            _randomNumberProvider.Setup(s => s.GetRandomInclusive(It.IsAny<int>(), It.IsAny<int>())).Returns(difficulty);

            _zombiePackDifficultyDirector.GetCalculatedZombiePackProgress(userId, zombiePackId);
            _randomNumberProvider.Verify(s => s.GetRandomInclusive(1, 4), Times.Exactly(2));
        }

        [TestMethod]
        public void should_return_a_hunt_count_of_5_to_10_if_resolved_as_an_medium_hotzone()
        {
            Guid userId = Guid.NewGuid();
            Guid zombiePackId = Guid.NewGuid();
            Guid lastVisitedHotZoneId = Guid.NewGuid();
            Guid hotzoneIdAssociatedWithZombiePack = lastVisitedHotZoneId;
            int baseLineAttackPower = 10;
            int difficulty = 2;

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(zombiePackId)).Returns(true);

            _userZombiePackProgressRetriever.Setup(s => s.GetUserZombiePackProgressFor(userId, zombiePackId))
                                            .Returns(() => null);

            _userRetriever.Setup(s => s.GetLastVisitedHotZone(userId)).Returns(lastVisitedHotZoneId);
            _zombiePackRetriever.Setup(s => s.GetHotZoneByZombiePackId(zombiePackId)).Returns(hotzoneIdAssociatedWithZombiePack);
            _userRetriever.Setup(s => s.GetAttackPowerForDifficultyCalculation(userId)).Returns(baseLineAttackPower).Verifiable();
            _randomNumberProvider.Setup(s => s.GetRandomInclusive(It.IsAny<int>(), It.IsAny<int>())).Returns(difficulty);

            _zombiePackDifficultyDirector.GetCalculatedZombiePackProgress(userId, zombiePackId);
            _randomNumberProvider.Verify(s => s.GetRandomInclusive(1, 4), Times.Once());
            _randomNumberProvider.Verify(s => s.GetRandomInclusive(5, 10), Times.Once());
        }

        [TestMethod]
        public void should_return_a_hunt_count_of_11_to_15_if_resolved_as_an_medium_hotzone()
        {
            Guid userId = Guid.NewGuid();
            Guid zombiePackId = Guid.NewGuid();
            Guid lastVisitedHotZoneId = Guid.NewGuid();
            Guid hotzoneIdAssociatedWithZombiePack = lastVisitedHotZoneId;
            int baseLineAttackPower = 10;
            int difficulty = 3;

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(zombiePackId)).Returns(true);

            _userZombiePackProgressRetriever.Setup(s => s.GetUserZombiePackProgressFor(userId, zombiePackId))
                                            .Returns(() => null);

            _userRetriever.Setup(s => s.GetLastVisitedHotZone(userId)).Returns(lastVisitedHotZoneId);
            _zombiePackRetriever.Setup(s => s.GetHotZoneByZombiePackId(zombiePackId)).Returns(hotzoneIdAssociatedWithZombiePack);
            _userRetriever.Setup(s => s.GetAttackPowerForDifficultyCalculation(userId)).Returns(baseLineAttackPower).Verifiable();
            _randomNumberProvider.Setup(s => s.GetRandomInclusive(It.IsAny<int>(), It.IsAny<int>())).Returns(difficulty);

            _zombiePackDifficultyDirector.GetCalculatedZombiePackProgress(userId, zombiePackId);
            _randomNumberProvider.Verify(s => s.GetRandomInclusive(1, 4), Times.Once());
            _randomNumberProvider.Verify(s => s.GetRandomInclusive(11, 15), Times.Once());
        }

        [TestMethod]
        public void should_return_a_hunt_count_of_16_to_20_if_resolved_as_an_medium_hotzone()
        {
            Guid userId = Guid.NewGuid();
            Guid zombiePackId = Guid.NewGuid();
            Guid lastVisitedHotZoneId = Guid.NewGuid();
            Guid hotzoneIdAssociatedWithZombiePack = lastVisitedHotZoneId;
            int baseLineAttackPower = 10;
            int difficulty = 4;

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(zombiePackId)).Returns(true);

            _userZombiePackProgressRetriever.Setup(s => s.GetUserZombiePackProgressFor(userId, zombiePackId))
                                            .Returns(() => null);

            _userRetriever.Setup(s => s.GetLastVisitedHotZone(userId)).Returns(lastVisitedHotZoneId);
            _zombiePackRetriever.Setup(s => s.GetHotZoneByZombiePackId(zombiePackId)).Returns(hotzoneIdAssociatedWithZombiePack);
            _userRetriever.Setup(s => s.GetAttackPowerForDifficultyCalculation(userId)).Returns(baseLineAttackPower).Verifiable();
            _randomNumberProvider.Setup(s => s.GetRandomInclusive(It.IsAny<int>(), It.IsAny<int>())).Returns(difficulty);

            _zombiePackDifficultyDirector.GetCalculatedZombiePackProgress(userId, zombiePackId);
            _randomNumberProvider.Verify(s => s.GetRandomInclusive(1, 4), Times.Once());
            _randomNumberProvider.Verify(s => s.GetRandomInclusive(16, 20), Times.Once());
        }
    }
}
