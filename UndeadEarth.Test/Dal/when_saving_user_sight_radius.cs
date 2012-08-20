using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Test.Dal.Utility;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_saving_user_sight_radius
    {
        private IUserSaver _userSaver;
        private string _connectionString;
        private Guid _userId;

        public when_saving_user_sight_radius()
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
        public void should_save_user_sight_radius()
        {
            _userSaver.SaveLastSightRadius(_userId, 20, DateTime.Today);
            TestDataContext dataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            var dto = dataContext.UserDtos
                                 .Single(u => u.Id == _userId);

            Assert.AreEqual(20, dto.LastSightRadius);
            Assert.AreEqual(DateTime.Today, dto.LastSightRadiusDate);
        }

        [TestMethod]
        public void should_save_user_max_energy()
        {
            _userSaver.SetCurrentBaseSightRadius(_userId, 300);
            TestDataContext dataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            var dto = dataContext.UserDtos
                                 .Single(u => u.Id == _userId);

            Assert.AreEqual(300, dto.BaseSightRadius);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_error_for_saving_last_energy_if_user_does_not_exist()
        {
            _userSaver.SaveLastSightRadius(Guid.NewGuid(), 100, DateTime.Today);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_error_for_saving_mas_energy_if_user_does_not_exist()
        {
            _userSaver.SetCurrentBaseSightRadius(Guid.NewGuid(), 100);
        }
    }
}
