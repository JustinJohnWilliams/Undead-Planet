using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;

namespace UndeadEarth.Test.Dal.Utility
{
    [TestClass]
    public class when_saving_user_energy
    {
        private IUserSaver _userSaver;
        private string _connectionString;
        private Guid _userId;

        public when_saving_user_energy()
        {
            _userSaver = DalTestContextSpecification.Instance.Resolve<IUserSaver>();
            _connectionString = DalTestContextSpecification.ConnectionString;
            _userId = Guid.NewGuid();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            TestDataContext dataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            UserDto userDto = new UserDto
            {
                Id = _userId,
                DisplayName = "name",
                Email = "email",
                Latitude = 0,
                LocationId = Guid.NewGuid(),
                Longitude = 0,
                ZoneId = Guid.NewGuid()
            };

            dataContext.UserDtos.InsertOnSubmit(userDto);

            dataContext.SubmitChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestDataContext dataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            dataContext.UserDtos
                       .DeleteAllOnSubmit(
                            dataContext.UserDtos
                                       .Where(u => u.Id == _userId));
            dataContext.SubmitChanges();
        }

        [TestMethod]
        public void should_save_user_last_energy()
        {
            _userSaver.SaveLastEnergy(_userId, 20, DateTime.Today);
            TestDataContext dataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            var dto = dataContext.UserDtos
                                 .Single(u => u.Id == _userId);

            Assert.AreEqual(20, dto.LastEnergy);
            Assert.AreEqual(DateTime.Today, dto.LastEnergyDate);
        }

        [TestMethod]
        public void should_save_user_max_energy()
        {
            _userSaver.UpdateEnergyForDifficultyCalculation(_userId, 300);
            TestDataContext dataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            var dto = dataContext.UserDtos
                                 .Single(u => u.Id == _userId);

            Assert.AreEqual(300, dto.BaseLineEnergy);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_error_for_saving_last_energy_if_user_does_not_exist()
        {
            _userSaver.SaveLastEnergy(Guid.NewGuid(), 100, DateTime.Today);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void should_throw_error_for_saving_mas_energy_if_user_does_not_exist()
        {
            _userSaver.UpdateEnergyForDifficultyCalculation(Guid.NewGuid(), 100);
        }
    }
}
