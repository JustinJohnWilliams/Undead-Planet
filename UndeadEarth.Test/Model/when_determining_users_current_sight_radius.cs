using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using Moq;
using UndeadEarth.Model;

namespace UndeadEarth.Test.Model
{
    [TestClass]
    public class when_determining_users_current_sight_radius
    {
        private IUserSightRadiusProvider _userSightRadiusProvider;
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IUserSaver> _userSaver;
        private Mock<IUserItemRetriever> _userItemRetriever;
        public when_determining_users_current_sight_radius()
        {
            _userRetriever = new Mock<IUserRetriever>();
            _userSaver = new Mock<IUserSaver>();
            _userItemRetriever = new Mock<IUserItemRetriever>();
            _userSightRadiusProvider = new UserEnergyAndSightProvider(_userRetriever.Object, _userSaver.Object, _userItemRetriever.Object);
        }

        [TestMethod]
        public void should_return_0_if_user_does_not_exist()
        {
            Guid userId = Guid.NewGuid();

            _userRetriever.Setup(s => s.UserExists(userId))
                          .Returns(false)
                          .Verifiable();

            Assert.AreEqual(0, _userSightRadiusProvider.GetUserSightRadius(userId, DateTime.Now));
            _userRetriever.Verify();
        }

        [TestMethod]
        public void should_return_min_sight_radius_for_user_if_user_exists_but_last_saved_sight_radius_does_not_exist()
        {
            Guid userId = Guid.NewGuid();

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            _userRetriever.Setup(s => s.GetLastSavedSightRadius(userId))
                          .Returns(null as Contract.Tuple<int, DateTime>)
                          .Verifiable();

            _userRetriever.Setup(s => s.GetCurrentBaseSightRadius(userId))
                          .Returns(5)
                          .Verifiable();

            Assert.AreEqual(5, _userSightRadiusProvider.GetUserSightRadius(userId, DateTime.Now));

            _userRetriever.Verify();
        }

        [TestMethod]
        public void should_set_last_saved_sight_radius_to_base_sight_radius_if_last_saved_radius_does_not_exist()
        {
            Guid userId = Guid.NewGuid();

            //given an time period
            DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

            //and a user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //with no last saved sight radius
            _userRetriever.Setup(s => s.GetLastSavedSightRadius(userId))
                           .Returns(null as Contract.Tuple<int, DateTime>)
                           .Verifiable();

            //and a specified base sight radius
            int? unsavedBaseEnergy = 10;
            _userRetriever.Setup(s => s.GetCurrentBaseSightRadius(userId)).Returns(() => unsavedBaseEnergy);

            //should return base sight radius
            Assert.AreEqual(10, _userSightRadiusProvider.GetUserSightRadius(userId, time));

            _userRetriever.Verify();
            _userSaver.Verify(s => s.SaveLastSightRadius(userId, 10, time));
        }

        [TestMethod]
        public void should_set_sight_radius_to_5_for_user_who_exists_but_does_not_have_a_base_sight_radius_value()
        {
            Guid userId = Guid.NewGuid();

            //given user that exist
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //with no last saved sight radius
            _userRetriever.Setup(s => s.GetLastSavedSightRadius(userId))
                          .Returns(null as Contract.Tuple<int, DateTime>)
                          .Verifiable();

            //and an unspecified base sight radius
            int? unsavedBaseSightRadius = null;
            _userRetriever.Setup(s => s.GetCurrentBaseSightRadius(userId)).Returns(() => unsavedBaseSightRadius);
            _userSaver.Setup(s => s.SetCurrentBaseSightRadius(userId, 5)).Callback<Guid, int>((passedInUserId, passedInEnergy) => unsavedBaseSightRadius = passedInEnergy);

            //should set sight radius to 5 as a starting point
            Assert.AreEqual(5, _userSightRadiusProvider.GetUserSightRadius(userId, DateTime.Now));

            _userRetriever.Verify();
            _userSaver.Verify(s => s.SetCurrentBaseSightRadius(userId, 5));
        }

        [TestMethod]
        public void should_return_last_sight_radius_saved_If_there_is_no_difference_between_the_current_time_passed_in_and_the_last_saved_sight_radius_from_the_repository()
        {
            Guid userId = Guid.NewGuid();

            //given a time with no elapsed minutes
            DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

            //and a user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //and an sight radius of 60 saved with no elapsed time
            _userRetriever.Setup(s => s.GetLastSavedSightRadius(userId))
                          .Returns(new UndeadEarth.Contract.Tuple<int, DateTime>(60, time));

            //and an unspecified sight radius
            int? unsavedBaseSightRadius = null;
            _userRetriever.Setup(s => s.GetCurrentBaseSightRadius(userId)).Returns(() => unsavedBaseSightRadius);
            _userSaver.Setup(s => s.SetCurrentBaseSightRadius(userId, 5)).Callback<Guid, int>((passedInUserId, passedInEnergy) => unsavedBaseSightRadius = passedInEnergy);

            //should return 60
            Assert.AreEqual(60, _userSightRadiusProvider.GetUserSightRadius(userId, time));
        }

        [TestMethod]
        public void should_truncate_seconds_and_milliseconds_from_times_that_are_being_used_for_calculating_sight_radius()
        {
            Guid userId = Guid.NewGuid();

            //given an ellapsed time of a second
            DateTime timeForRetriever = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            DateTime timeForCalculation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 59);

            //and a user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //and a last saved sight radius of 60 one second ago
            _userRetriever.Setup(s => s.GetLastSavedSightRadius(userId))
                          .Returns(new UndeadEarth.Contract.Tuple<int, DateTime>(60, timeForRetriever));

            //and an unspecified base energy
            int? unsavedSightRadius = null;
            _userRetriever.Setup(s => s.GetCurrentBaseSightRadius(userId)).Returns(() => unsavedSightRadius);
            _userSaver.Setup(s => s.SetCurrentBaseSightRadius(userId, 5)).Callback<Guid, int>((passedInUserId, passedInEnergy) => unsavedSightRadius = passedInEnergy);

            //should return 60 because a second isn't enough
            Assert.AreEqual(60, _userSightRadiusProvider.GetUserSightRadius(userId, timeForCalculation));
        }

        [TestMethod]
        public void should_return_0_if_time_passed_into_calculate_sight_radius_is_less_than_the_last_saved_sight_radius_record()
        {
            Guid userId = Guid.NewGuid();
            DateTime timeForRetriever = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            DateTime timeForCalculation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 59);
            timeForCalculation = timeForCalculation.AddDays(-1);

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            _userRetriever.Setup(s => s.GetLastSavedSightRadius(userId))
                          .Returns(new UndeadEarth.Contract.Tuple<int, DateTime>(60, timeForRetriever));

            Assert.AreEqual(0, _userSightRadiusProvider.GetUserSightRadius(userId, timeForCalculation));
        }

        [TestMethod]
        public void should_return_calcuated_sight_radius_for_user_based_on_minutes_passed_since_last_saved_sight_radius()
        {
            Guid userId = Guid.NewGuid();

            //given an elapsed time of 10 minutes
            DateTime timeForRetriever = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            DateTime timeForCalculation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 59);
            timeForCalculation = timeForCalculation.AddMinutes(10);

            //and an existing user
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //and a last saved sight radius of 60
            _userRetriever.Setup(s => s.GetLastSavedSightRadius(userId))
                          .Returns(new UndeadEarth.Contract.Tuple<int, DateTime>(60, timeForRetriever));

            //and an unspecified base sight radius
            int? unsavedBaseSightRadius = null;
            _userRetriever.Setup(s => s.GetCurrentBaseSightRadius(userId)).Returns(() => unsavedBaseSightRadius);
            _userSaver.Setup(s => s.SetCurrentBaseSightRadius(userId, 5)).Callback<Guid, int>((passedInUserId, passedInEnergy) => unsavedBaseSightRadius = passedInEnergy);

            Assert.AreEqual(50, _userSightRadiusProvider.GetUserSightRadius(userId, timeForCalculation));
        }

        [TestMethod]
        public void should_return_min_sight_radius_if_calculated_sight_for_time_passed_in_is_less_than_min_sight_radius_of_user()
        {
            Guid userId = Guid.NewGuid();

            //given an elpased time of 300 minutes
            DateTime timeForRetriever = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            DateTime timeForCalculation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 59);
            timeForCalculation = timeForCalculation.AddMinutes(300);

            //and a user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //and an sight radius of 60, 300 minutes ago
            _userRetriever.Setup(s => s.GetLastSavedSightRadius(userId))
                          .Returns(new UndeadEarth.Contract.Tuple<int, DateTime>(60, timeForRetriever));

            //and an unspecified base sight radius
            int? unsavedBaseSightRadius = null;
            _userRetriever.Setup(s => s.GetCurrentBaseSightRadius(userId)).Returns(() => unsavedBaseSightRadius);
            _userSaver.Setup(s => s.SetCurrentBaseSightRadius(userId, 5)).Callback<Guid, int>((passedInUserId, passedInEnergy) => unsavedBaseSightRadius = passedInEnergy);

            //should return 5, because minimum cannot be exceeded
            Assert.AreEqual(5, _userSightRadiusProvider.GetUserSightRadius(userId, timeForCalculation));
        }

        [TestMethod]
        public void should_take_into_consideration_equipped_items_when_determining_min_sight_radius()
        {
            Guid userId = Guid.NewGuid();

            //given a long period of time
            DateTime timeForRetriever = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            DateTime timeForCalculation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 59);
            timeForCalculation = timeForCalculation.AddMinutes(300);

            //and a user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //with a last saved sight radius close to the minimum
            _userRetriever.Setup(s => s.GetLastSavedSightRadius(userId))
                          .Returns(new UndeadEarth.Contract.Tuple<int, DateTime>(5, timeForRetriever));


            //and the following equipment, one of which is one time use, one of which is not
            List<IItem> items = new List<IItem>();

            Mock<IItem> item = new Mock<IItem>();
            item.SetupGet(s => s.Distance).Returns(20);
            item.SetupGet(s => s.IsOneTimeUse).Returns(true);
            items.Add(item.Object);

            item = new Mock<IItem>();
            item.SetupGet(s => s.Distance).Returns(10);
            item.SetupGet(s => s.IsOneTimeUse).Returns(false);
            items.Add(item.Object);
            _userItemRetriever.Setup(s => s.GetUserItems(userId)).Returns(items);

            //and unspecified base sight radius
            int? unsavedBaseSightRadius = null;
            _userRetriever.Setup(s => s.GetCurrentBaseSightRadius(userId)).Returns(() => unsavedBaseSightRadius);
            _userSaver.Setup(s => s.SetCurrentBaseSightRadius(userId, 5)).Callback<Guid, int>((passedInUserId, passedInEnergy) => unsavedBaseSightRadius = passedInEnergy);

            //should return the base sight radius plus the combined sight radius of all equipment excluding one time use
            Assert.AreEqual(5 + 10, _userSightRadiusProvider.GetUserSightRadius(userId, timeForCalculation));
        }
    }
}
