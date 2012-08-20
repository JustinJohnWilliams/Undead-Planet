using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Test.Dal.Utility;
using UndeadEarth.Model;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_saving_user_zombie_pack_progress
    {
        private IUserZombiePackProgressSaver _userZombiePackProgressSaver;

        private Guid _userId;
        private Guid _zombiePackId;
        private string _connectionString;

        public when_saving_user_zombie_pack_progress()
        {
            _userZombiePackProgressSaver = DalTestContextSpecification.Instance.Resolve<IUserZombiePackProgressSaver>();
            
            _userId = Guid.NewGuid();
            _zombiePackId = Guid.NewGuid();
            _connectionString = DalTestContextSpecification.ConnectionString;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                dataContext.UserZombiePackProgressDtos.DeleteAllOnSubmit(
                                dataContext.UserZombiePackProgressDtos.Where(c => c.UserId == _userId));

                dataContext.SubmitChanges();
            }
        }

        [TestMethod]
        public void should_insert_zombie_pack_progress_that_does_exist()
        {
            UserZombiePackProgress progress = new UserZombiePackProgress { MaxZombies = 100, IsDestroyed = true, ZombiesLeft = 25 };
            _userZombiePackProgressSaver.SaveZombiePackProgress(_userId, _zombiePackId, progress);
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                var dto = dataContext.UserZombiePackProgressDtos.Single(s => s.UserId == _userId && s.ZombiePackId == _zombiePackId);
                Assert.AreEqual(_userId, dto.UserId);
                Assert.AreEqual(_zombiePackId, dto.ZombiePackId);
                Assert.AreEqual(100, dto.MaxZombies);
                Assert.AreEqual(25, dto.ZombiesLeft);
                Assert.AreEqual(true, dto.IsDestroyed);
            }
        }

        [TestMethod]
        public void should_update_existing_zombie_pack_progress()
        {
            UserZombiePackProgress progress = new UserZombiePackProgress { MaxZombies = 100, IsDestroyed = true, ZombiesLeft = 25 };
            _userZombiePackProgressSaver.SaveZombiePackProgress(_userId, _zombiePackId, progress);

            //update
            progress.ZombiesLeft = 10;
            progress.MaxZombies = 20;
            progress.IsDestroyed = false;
            _userZombiePackProgressSaver.SaveZombiePackProgress(_userId, _zombiePackId, progress);

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                var dto = dataContext.UserZombiePackProgressDtos.Single(s => s.UserId == _userId && s.ZombiePackId == _zombiePackId);
                Assert.AreEqual(_userId, dto.UserId);
                Assert.AreEqual(_zombiePackId, dto.ZombiePackId);
                Assert.AreEqual(20, dto.MaxZombies);
                Assert.AreEqual(10, dto.ZombiesLeft);
                Assert.AreEqual(false, dto.IsDestroyed);
            }
        }
    }
}
