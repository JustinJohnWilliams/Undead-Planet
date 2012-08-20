using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Test.Dal.Utility;

namespace UndeadEarth.Test.Dal
{
    /// <summary>
    /// Summary description for when_retrieving_user_energy
    /// </summary>
    [TestClass]
    public class when_retrieving_user_energy
    {
        private IUserRetriever _userRetriever;
        private Guid _populatedUserId;
        private Guid _emptyUserId;
        public when_retrieving_user_energy()
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
                LastEnergy = 50,
                LastEnergyDate = DateTime.Today,
                Latitude = 0,
                LocationId = Guid.NewGuid(),
                Longitude = 0,
                BaseLineEnergy = 100,
                ZoneId = Guid.NewGuid()
            };

            dataContext.UserDtos.InsertOnSubmit(userDto);

            userDto = new UserDto
            {
                Id = _emptyUserId,
                DisplayName = "name",
                Email = "email",
                LastEnergy = null,
                LastEnergyDate = null,
                Latitude = 0,
                LocationId = Guid.NewGuid(),
                Longitude = 0,
                BaseLineEnergy = 0,
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
        public void should_return_null_for_last_saved_energy_if_user_does_not_exist()
        {
            Assert.IsNull(_userRetriever.GetLastSavedEnergy(Guid.NewGuid()));
        }

        [TestMethod]
        public void should_return_null_for_last_saved_energy_if_user_exists_but_energy_is_null()
        {
            Assert.IsNull(_userRetriever.GetLastSavedEnergy(_emptyUserId));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_error_if_user_does_not_exist()
        {
            Assert.AreEqual(0, _userRetriever.GetEnergyForDifficultyCalculation(Guid.NewGuid()));
        }

        [TestMethod]
        public void should_return_0_for_base_energy()
        {
            Assert.AreEqual(0, _userRetriever.GetEnergyForDifficultyCalculation(_emptyUserId));
        }

        [TestMethod]
        public void should_return_last_saved_energy_for_user()
        {
            Contract.Tuple<int, DateTime> energy = _userRetriever.GetLastSavedEnergy(_populatedUserId);
            Assert.AreEqual(50, energy.Item1);
            Assert.AreEqual(DateTime.Today, energy.Item2);
        }

        [TestMethod]
        public void should_return_base_energy_for_user()
        {
            Assert.AreEqual(100, _userRetriever.GetEnergyForDifficultyCalculation(_populatedUserId));
        }

        [TestMethod]
        public void should_set_last_energy_date_to_datetimenow_if_its_null_but_last_energy_isnt()
        {
            TestDataContext dataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            var dto = dataContext.UserDtos.Single(s => s.Id == _emptyUserId);
            dto.LastEnergy = 80;
            dataContext.SubmitChanges();

            Contract.Tuple<int, DateTime> energy = _userRetriever.GetLastSavedEnergy(_emptyUserId);
            Assert.AreEqual(80, energy.Item1);
            Assert.AreEqual(DateTime.Now.ToString(), energy.Item2.ToString());
        }
    }
}
