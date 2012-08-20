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
    public class when_retrieving_user_sight_radius
    {
        private IUserRetriever _userRetriever;
        private Guid _populatedUserId;
        private Guid _emptyUserId;
        public when_retrieving_user_sight_radius()
        {
            _userRetriever = DalTestContextSpecification.Instance.Resolve<IUserRetriever>();
            _populatedUserId = Guid.NewGuid();
            _emptyUserId = Guid.NewGuid();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            TestDataContext dataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            UserDto userDto = new UserDto
            {
                Id = _populatedUserId,
                DisplayName = "name",
                Email = "email",
                LastSightRadius = 50,
                LastSightRadiusDate = DateTime.Today,
                Latitude = 0,
                LocationId = Guid.NewGuid(),
                Longitude = 0,
                BaseSightRadius = 100,
                ZoneId = Guid.NewGuid()
            };

            dataContext.UserDtos.InsertOnSubmit(userDto);

            userDto = new UserDto
            {
                Id = _emptyUserId,
                DisplayName = "name",
                Email = "email",
                LastSightRadius = null,
                LastSightRadiusDate = null,
                Latitude = 0,
                LocationId = Guid.NewGuid(),
                Longitude = 0,
                BaseSightRadius = null,
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
                                       .Where(u => u.Id == _emptyUserId
                                                   || u.Id == _populatedUserId));
            dataContext.SubmitChanges();
        }


        [TestMethod]
        public void should_return_null_for_last_saved_sight_radius_if_user_does_not_exist()
        {
            Assert.IsNull(_userRetriever.GetLastSavedSightRadius(Guid.NewGuid()));
        }

        [TestMethod]
        public void should_return_null_for_last_saved_sight_radius_if_user_exists_but_sight_radius_is_null()
        {
            Assert.IsNull(_userRetriever.GetLastSavedSightRadius(_emptyUserId));
        }

        [TestMethod]
        public void should_return_null_for_base_sight_radius_if_user_does_not_exist()
        {
            Assert.IsNull(_userRetriever.GetCurrentBaseSightRadius(Guid.NewGuid()));
        }

        [TestMethod]
        public void should_return_null_for_base_sight_radius_if_user_exists_but_max_sight_radius_is_null()
        {
            Assert.IsNull(_userRetriever.GetCurrentBaseSightRadius(_emptyUserId));
        }

        [TestMethod]
        public void should_return_last_saved_sight_radius_for_user()
        {
            Contract.Tuple<int, DateTime> sightRadius = _userRetriever.GetLastSavedSightRadius(_populatedUserId);
            Assert.AreEqual(50, sightRadius.Item1);
            Assert.AreEqual(DateTime.Today, sightRadius.Item2);
        }

        [TestMethod]
        public void should_return_base_sight_radius_for_user()
        {
            Assert.AreEqual(100, _userRetriever.GetCurrentBaseSightRadius(_populatedUserId));
        }

        [TestMethod]
        public void should_set_last_sight_radius_date_to_datetimenow_if_its_null_but_last_sight_radius_isnt()
        {
            TestDataContext dataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            var dto = dataContext.UserDtos.Single(s => s.Id == _emptyUserId);
            dto.LastSightRadius = 80;
            dataContext.SubmitChanges();

            Contract.Tuple<int, DateTime> sightRadius = _userRetriever.GetLastSavedSightRadius(_emptyUserId);
            Assert.AreEqual(80, sightRadius.Item1);
            Assert.AreEqual(DateTime.Now.ToString(), sightRadius.Item2.ToString());
        }
    }
}
