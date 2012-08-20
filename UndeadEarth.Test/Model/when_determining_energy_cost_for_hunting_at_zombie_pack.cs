using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UndeadEarth.Contract;
using UndeadEarth.Model;

namespace UndeadEarth.Test.Model
{
    [TestClass]
    public class when_determining_energy_cost_for_hunting_at_zombie_pack
    {
        private Guid _hotzoneIdAssociatedWithZombiePack;
        private Guid _lastVisitedHotZoneId;
        private Guid _userId;
        private Mock<IUserRetriever> _userRetriever;
        private Guid _zombiePackId;
        private Mock<IZombiePackRetriever> _zombiePackRetriever;
        private Mock<IUserZombiePackProgressRetriever> _userZombiePackProgressRetriever;
        private Mock<IRandomNumberProvider> _randomNumberProvider;
        private Mock<IUserSaver> _userSaver;
        private Mock<IUserPotentialProvider> _userPotentialProvider;
        private Mock<IUserZombiePackProgressSaver> _userZombiePackProgressSaver;
        private IZombiePackDifficultyDirector _zombiePackDifficultyDirector;
        private Mock<IUserStatsRetriever> _userStatsRetriever;

        public when_determining_energy_cost_for_hunting_at_zombie_pack()
        {
            
        }

        [TestInitialize]
        public void Setup()
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

            _userId = Guid.NewGuid();
            _zombiePackId = Guid.NewGuid();
            _lastVisitedHotZoneId = Guid.NewGuid();
            _hotzoneIdAssociatedWithZombiePack = Guid.NewGuid();

            GivenUserExists();
            GivenZombiePackExists();
            GivenUserHasNotVisitedZombiePack();
            GivenUserHasNotVisitedThisHotZone();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _userRetriever.Verify();
            _zombiePackRetriever.Verify();
            _userPotentialProvider.Verify();
            _userPotentialProvider.Verify();
        }

        [TestMethod]
        public void should_determine_base_line_for_user_that_changes_hotzones_after_each_third_hotzone()
        {
            int maxPotentialUserAttackPower = 10;
            int maxPotentialUserEnergy = 100;

            GivenMaxPotentialAttackAndEnergy(maxPotentialUserAttackPower, maxPotentialUserEnergy);

            GivenUserHasDetroyedHotZones(count: 3);

            _zombiePackDifficultyDirector.GetEnergyCost(_userId, _zombiePackId);

            ShouldHaveIncreasedDifficultyOfZombiePacks(maxPotentialUserAttackPower, maxPotentialUserEnergy);
        }

        [TestMethod]
        public void should_return_energy_cost_at_5_percent_of_maximum_potential()
        {
            int baseLineEnergy = 100;

            GivenMaxPotentialAttackAndEnergy(0, baseLineEnergy);

            GivenUserHasDetroyedHotZones(count: 3);

            int energyCost = _zombiePackDifficultyDirector.GetEnergyCost(_userId, _zombiePackId);

            Assert.AreEqual(Convert.ToInt32(baseLineEnergy * .05), energyCost);
        }

        private void GivenUserExists()
        {
            _userRetriever.Setup(s => s.UserExists(_userId)).Returns(true);
        }

        private void GivenZombiePackExists()
        {
            _zombiePackRetriever.Setup(s => s.ZombiePackExists(_zombiePackId)).Returns(true);
            _zombiePackRetriever.Setup(s => s.GetHotZoneByZombiePackId(_zombiePackId)).Returns(_hotzoneIdAssociatedWithZombiePack).Verifiable();
        }

        private void GivenUserHasNotVisitedZombiePack()
        {
            _userZombiePackProgressRetriever.Setup(s => s.GetUserZombiePackProgressFor(_userId, _zombiePackId))
                                                        .Returns(() => null);
        }

        private void GivenUserHasNotVisitedThisHotZone()
        {
            _userRetriever.Setup(s => s.GetLastVisitedHotZone(_userId)).Returns(_lastVisitedHotZoneId).Verifiable();
        }

        private void GivenMaxPotentialAttackAndEnergy(int attack, int energy)
        {
            _userPotentialProvider.Setup(s => s.GetMaxPotentialAttackPower(_userId)).Returns(attack).Verifiable();
            _userPotentialProvider.Setup(s => s.GetMaxPotentialEnergy(_userId)).Returns(energy).Verifiable();
            _userRetriever.Setup(s => s.GetAttackPowerForDifficultyCalculation(_userId)).Returns(attack);
            _userRetriever.Setup(s => s.GetEnergyForDifficultyCalculation(_userId)).Returns(energy);
        }

        private void ShouldHaveIncreasedDifficultyOfZombiePacks(int expectedAttack, int expectedEnergy)
        {
            _userSaver.Verify(s => s.UpdateBaseLine(_userId, expectedAttack, expectedEnergy));
            
        }
        private void GivenUserHasDetroyedHotZones(int count)
        {
            _userStatsRetriever.Setup(s => s.GetStats(_userId)).Returns(new UserStats { HotZonesDestroyed = count });
        }
    }
}
