using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Test.Dal.Utility;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_determining_if_hotzones_is_cleared
    {
        private List<Guid> _hotZonesIds;
        private List<Guid> _zombiePackIds;
        private IHotZoneRetriever _hotZoneRetriever;
        private List<Guid> _userIds;

        public when_determining_if_hotzones_is_cleared()
        {
            
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _hotZonesIds = new List<Guid>();
            _zombiePackIds = new List<Guid>();
            _userIds = new List<Guid>();
            _hotZoneRetriever = DalTestContextSpecification.Instance.Resolve<IHotZoneRetriever>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestDataContext testDataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            testDataContext.HotZoneDtos.DeleteAllOnSubmit(testDataContext.HotZoneDtos.Where(h => _hotZonesIds.Contains(h.Id)));
            testDataContext.SubmitChanges();
            testDataContext.ZombiePackDtos.DeleteAllOnSubmit(testDataContext.ZombiePackDtos.Where(z => _zombiePackIds.Contains(z.Id)));
            testDataContext.SubmitChanges();
            testDataContext.UserZombiePackProgressDtos.DeleteAllOnSubmit(testDataContext.UserZombiePackProgressDtos.Where(z => _zombiePackIds.Contains(z.ZombiePackId)));
            testDataContext.SubmitChanges();
            testDataContext.UserDtos.DeleteAllOnSubmit(testDataContext.UserDtos.Where(u => _userIds.Contains(u.Id)));
            testDataContext.SubmitChanges();

        }

        [TestMethod]
        public void should_return_hotzone_as_cleared()
        {
            Guid hotzoneId = GivenHotZone("Test Hotzone");
            Guid zombiePack1 = GivenZombiePackInHotZone(hotzoneId, "Zombie Pack 1");
            Guid zombePack2 = GivenZombiePackInHotZone(hotzoneId, "Zombe Pack 2");
            Guid userId = GivenUser("John Doe");

            GivenAllZombiesDestroyedIn(userId, zombiePack1);
            GivenAllZombiesDestroyedIn(userId, zombePack2);

            TheHotZoneShouldBeCleared(userId, hotzoneId);
        }

        [TestMethod]
        public void should_return_hotzone_is_not_cleared_for_other_user()
        {
            Guid hotzoneId = GivenHotZone("Test Hotzone");
            Guid zombiePack1 = GivenZombiePackInHotZone(hotzoneId, "Zombie Pack 1");
            Guid zombePack2 = GivenZombiePackInHotZone(hotzoneId, "Zombe Pack 2");
            Guid userId = GivenUser("John Doe");
            Guid otherUserId = GivenUser("Jane Doe");

            GivenAllZombiesDestroyedIn(userId, zombiePack1);
            GivenAllZombiesDestroyedIn(userId, zombePack2);

            TheHotZoneShouldBeCleared(userId, hotzoneId);
            TheHotZoneShouldNotBeCleared(otherUserId, hotzoneId);
        }

        private Guid GivenHotZone(string name)
        {
            TestDataContext testDataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            Guid hotZoneId = Guid.NewGuid();
            HotZoneDto hotZoneDto = new HotZoneDto
            {
                Id = hotZoneId,
                CanStartHere = false,
                Latitude = 0,
                Longitude = 0,
                Name = name
            };

            _hotZonesIds.Add(hotZoneId);

            testDataContext.HotZoneDtos.InsertOnSubmit(hotZoneDto);
            testDataContext.SubmitChanges();

            return hotZoneId;
        }

        private Guid GivenZombiePackInHotZone(Guid hotzoneId, string name)
        {
            TestDataContext testDataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);

            Guid zombiePackId = Guid.NewGuid();
            _zombiePackIds.Add(zombiePackId);
            ZombiePackDto zombiePackDto = new ZombiePackDto
            {
                HotZoneId = hotzoneId,
                Id = zombiePackId,
                Latitude = 0,
                Longitude = 0,
                Name = name
            };

            testDataContext.ZombiePackDtos.InsertOnSubmit(zombiePackDto);
            testDataContext.SubmitChanges();
            return zombiePackId;
        }

        private Guid GivenUser(string name)
        {
            TestDataContext testDataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            Guid userId = Guid.NewGuid();
            UserDto userDto = new UserDto
            {
                Id = userId,
                BaseLineAttackPower = 0,
                BaseLineEnergy = 0,
                CurrentBaseAttack = 0,
                CurrentBaseEnergy = 0,
                DisplayName = name,
                Email = string.Empty,
                FacebookUserId = 0,
                Latitude = 0,
                Level = 0,
                ZoneId = Guid.Empty
            };

            testDataContext.UserDtos.InsertOnSubmit(userDto);
            testDataContext.SubmitChanges();
            _userIds.Add(userId);

            return userId;
        }

        private void GivenAllZombiesDestroyedIn(Guid userId, Guid zombiePackId)
        {
            TestDataContext testDataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            UserZombiePackProgressDto userZombiePackProgressDto = new UserZombiePackProgressDto
            {
                Id = Guid.NewGuid(),
                IsDestroyed = true,
                LastHuntDate = DateTime.Today,
                LastRegen = DateTime.Today,
                MaxZombies = 5,
                RegenMinuteTicks = 10,
                RegenZombieRate = 10,
                UserId = userId,
                ZombiePackId = zombiePackId,
                ZombiesLeft = 0
            };

            testDataContext.UserZombiePackProgressDtos.InsertOnSubmit(userZombiePackProgressDto);
            testDataContext.SubmitChanges();
        }

        private void TheHotZoneShouldBeCleared(Guid userId, Guid hotzoneId)
        {
            IHotZoneRetriever hotZoneRetriever = DalTestContextSpecification.Instance.Resolve<IHotZoneRetriever>();
            IZombiePackRetriever zombiePackRetriever = DalTestContextSpecification.Instance.Resolve<IZombiePackRetriever>();

            List<KeyValuePair<Guid, int>> zombiePackCounts = hotZoneRetriever.GetRemainingZombiePacksInHotZones(userId);

            Assert.IsFalse(zombiePackCounts.Any(z => z.Key == hotzoneId), "Hot zone was contained in list.");

            Assert.AreEqual(0, _hotZoneRetriever.ZombiePacksLeft(userId, hotzoneId), "Zombie packs left was not zero.");
        }

        private void TheHotZoneShouldNotBeCleared(Guid userId, Guid hotzoneId)
        {
            IHotZoneRetriever hotZoneRetriever = DalTestContextSpecification.Instance.Resolve<IHotZoneRetriever>();
            IZombiePackRetriever zombiePackRetriever = DalTestContextSpecification.Instance.Resolve<IZombiePackRetriever>();

            List<KeyValuePair<Guid, int>> zombiePackCounts = hotZoneRetriever.GetRemainingZombiePacksInHotZones(userId);

            Assert.IsTrue(zombiePackCounts.Any(z => z.Key == hotzoneId && z.Value == 2), "Hotzone was not in list.");

            Assert.AreEqual(2, _hotZoneRetriever.ZombiePacksLeft(userId, hotzoneId), "Zombie packs left was not what we expected.");
        }
    }
}
