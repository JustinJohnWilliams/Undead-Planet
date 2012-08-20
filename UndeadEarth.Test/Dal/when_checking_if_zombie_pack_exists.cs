using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Dal;
using UndeadEarth.Test.Dal.Utility;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_checking_if_zombie_pack_exists
    {
        private IZombiePackRetriever _zombiePackRetriever;
        private Guid _zombiePackId;
        public when_checking_if_zombie_pack_exists()
        {
            _zombiePackRetriever = new ZombiePackRepository(DalTestContextSpecification.ConnectionString, null);
            _zombiePackId = Guid.NewGuid();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            TestDataContext dataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            ZombiePackDto dto = new ZombiePackDto
            {
                Id = _zombiePackId,
                HotZoneId = Guid.NewGuid(),
                Latitude = 0,
                Longitude = 0,
                Name = string.Empty
            };

            dataContext.ZombiePackDtos.InsertOnSubmit(dto);
            dataContext.SubmitChanges();
        }

        [TestMethod]
        public void should_return_false_if_zombiepack_does_not_exist()
        {
            Assert.IsFalse(_zombiePackRetriever.ZombiePackExists(Guid.NewGuid()));
        }

        [TestMethod]
        public void should_return_true_if_zombiepack_exists()
        {
            Assert.IsTrue(_zombiePackRetriever.ZombiePackExists(_zombiePackId));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestDataContext dataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            dataContext.ZombiePackDtos.DeleteOnSubmit(dataContext.ZombiePackDtos.Single(s => s.Id == _zombiePackId));
            dataContext.SubmitChanges();
        }
    }
}
