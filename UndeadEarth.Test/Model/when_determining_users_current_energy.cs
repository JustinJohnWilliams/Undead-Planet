using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Model;
using Moq;

namespace UndeadEarth.Test.Model
{
    [TestClass]
    public class when_determining_users_current_energy
    {
        private IUserEnergyProvider _userEnergyProvider;
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IUserSaver> _userSaver;
        private Mock<IUserItemRetriever> _userItemRetriever;
        public when_determining_users_current_energy()
        {
            _userRetriever = new Mock<IUserRetriever>();
            _userSaver = new Mock<IUserSaver>();
            _userItemRetriever = new Mock<IUserItemRetriever>();
            _userEnergyProvider = new UserEnergyAndSightProvider(_userRetriever.Object, _userSaver.Object, _userItemRetriever.Object);
        }

        [TestMethod]
        public void should_return_0_if_user_does_not_exist()
        {
            Guid userId = Guid.NewGuid();

            _userRetriever.Setup(s => s.UserExists(userId))
                          .Returns(false)
                          .Verifiable();

            Assert.AreEqual(0, _userEnergyProvider.GetUserEnergy(userId, DateTime.Now));
            _userRetriever.Verify();
        }

        [TestMethod]
        public void should_return_max_energy_for_user_if_user_exists_but_last_saved_energy_does_not_exist()
        {
            Guid userId = Guid.NewGuid();

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            _userRetriever.Setup(s => s.GetLastSavedEnergy(userId))
                          .Returns(null as Contract.Tuple<int, DateTime>)
                          .Verifiable();

            _userRetriever.Setup(s => s.GetCurrentBaseEnergy(userId))
                          .Returns(100)
                          .Verifiable();

            Assert.AreEqual(100, _userEnergyProvider.GetUserEnergy(userId, DateTime.Now));

            _userRetriever.Verify();
        }

        [TestMethod]
        public void should_set_last_saved_energy_to_max_energy_if_last_saved_energy_does_not_exist()
        {
            Guid userId = Guid.NewGuid();

            //given an time period
            DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

            //and a user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //with no last saved energy
            _userRetriever.Setup(s => s.GetLastSavedEnergy(userId))
                           .Returns(null as Contract.Tuple<int, DateTime>)
                           .Verifiable();

            //and an specified base energy
            int unsavedBaseEnergy = 100;
            _userRetriever.Setup(s => s.GetCurrentBaseEnergy(userId)).Returns(() => unsavedBaseEnergy);

            //should return base energy of 100
            Assert.AreEqual(100, _userEnergyProvider.GetUserEnergy(userId, time));
            
            _userRetriever.Verify();
            _userSaver.Verify(s => s.SaveLastEnergy(userId, 100, time));
        }

        [TestMethod]
        public void should_set_energy_to_100_for_user_who_exists_but_does_not_have_a_max_energy_value()
        {
            Guid userId = Guid.NewGuid();

            //given user that exist
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //with no last saved energy
            _userRetriever.Setup(s => s.GetLastSavedEnergy(userId))
                          .Returns(null as Contract.Tuple<int, DateTime>)
                          .Verifiable();

            //and an unspecified base energy
            int unsavedBaseEnergy = 0;
            _userRetriever.Setup(s => s.GetCurrentBaseEnergy(userId)).Returns(() => unsavedBaseEnergy);
            _userSaver.Setup(s => s.UpdateCurrentBaseEnergy(userId, 100)).Callback<Guid, int>((passedInUserId, passedInEnergy) => unsavedBaseEnergy = passedInEnergy);
            
            //should set energy to 100 as a starting point
            Assert.AreEqual(100, _userEnergyProvider.GetUserEnergy(userId, DateTime.Now));

            _userRetriever.Verify();
            _userSaver.Verify(s => s.UpdateCurrentBaseEnergy(userId, 100));
        }

        [TestMethod]
        public void should_return_last_energy_saved_If_there_is_no_difference_between_the_current_time_passed_in_and_the_last_saved_energy_from_the_repository()
        {
            Guid userId = Guid.NewGuid();

            //given a time with no elapsed minutes
            DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

            //and a user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //and an energy of 60 saved with no elapsed time
            _userRetriever.Setup(s => s.GetLastSavedEnergy(userId))
                          .Returns(new UndeadEarth.Contract.Tuple<int, DateTime>(60, time));

            //and an unspecified base energy
            int unsavedBaseEnergy = 0;
            _userRetriever.Setup(s => s.GetCurrentBaseEnergy(userId)).Returns(() => unsavedBaseEnergy);
            _userSaver.Setup(s => s.UpdateCurrentBaseEnergy(userId, 100)).Callback<Guid, int>((passedInUserId, passedInEnergy) => unsavedBaseEnergy = passedInEnergy);

            //should return 60
            Assert.AreEqual(60, _userEnergyProvider.GetUserEnergy(userId, time));
        }

        [TestMethod]
        public void should_truncate_seconds_and_milliseconds_from_times_that_are_being_used_for_calculating_energy()
        {
            Guid userId = Guid.NewGuid();
           
            //given an ellapsed time of a seconde
            DateTime timeForRetriever = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            DateTime timeForCalculation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 59);

            //and a user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //and a last saved energy of 60 on seconed ago
            _userRetriever.Setup(s => s.GetLastSavedEnergy(userId))
                          .Returns(new UndeadEarth.Contract.Tuple<int, DateTime>(60, timeForRetriever));

            //and an unspecified base energy
            int unsavedBaseEnergy = 0;
            _userRetriever.Setup(s => s.GetCurrentBaseEnergy(userId)).Returns(() => unsavedBaseEnergy);
            _userSaver.Setup(s => s.UpdateCurrentBaseEnergy(userId, 100)).Callback<Guid, int>((passedInUserId, passedInEnergy) => unsavedBaseEnergy = passedInEnergy);

            //should return 60 because a second isn't enough
            Assert.AreEqual(60, _userEnergyProvider.GetUserEnergy(userId, timeForCalculation));
        }

        [TestMethod]
        public void should_return_0_if_time_passed_into_calculate_energy_is_less_than_the_last_saved_energy_record()
        {
            Guid userId = Guid.NewGuid();
            DateTime timeForRetriever = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            DateTime timeForCalculation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 59);
            timeForCalculation = timeForCalculation.AddDays(-1);

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            _userRetriever.Setup(s => s.GetLastSavedEnergy(userId))
                          .Returns(new UndeadEarth.Contract.Tuple<int, DateTime>(60, timeForRetriever));

            Assert.AreEqual(0, _userEnergyProvider.GetUserEnergy(userId, timeForCalculation));
        }

        [TestMethod]
        public void should_return_calcuated_energy_for_user_based_on_minutes_passed_since_last_saved_energy()
        {
            Guid userId = Guid.NewGuid();

            //given an elapsed time of 1 minute
            DateTime timeForRetriever = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            DateTime timeForCalculation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 59);
            timeForCalculation = timeForCalculation.AddMinutes(1);

            //and an existing user
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //and a last saved energy of 60
            _userRetriever.Setup(s => s.GetLastSavedEnergy(userId))
                          .Returns(new UndeadEarth.Contract.Tuple<int, DateTime>(60, timeForRetriever));

            //and an unspecified base energy
            int unsavedBaseEnergy = 0;
            _userRetriever.Setup(s => s.GetCurrentBaseEnergy(userId)).Returns(() => unsavedBaseEnergy);
            _userSaver.Setup(s => s.UpdateCurrentBaseEnergy(userId, 100)).Callback<Guid, int>((passedInUserId, passedInEnergy) => unsavedBaseEnergy = passedInEnergy);

            Assert.AreEqual(70 /* elapsed minutes multiplied by 10 */, _userEnergyProvider.GetUserEnergy(userId, timeForCalculation));
        }

        [TestMethod]
        public void should_return_max_energy_if_calculated_energy_for_time_passed_in_is_greater_than_max_energy_of_user()
        {
            Guid userId = Guid.NewGuid();

            //given an elpased time of 300 minutes
            DateTime timeForRetriever = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            DateTime timeForCalculation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 59);
            timeForCalculation = timeForCalculation.AddMinutes(300);

            //and a user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //and an energy of 60, 300 minutes ago
            _userRetriever.Setup(s => s.GetLastSavedEnergy(userId))
                          .Returns(new UndeadEarth.Contract.Tuple<int, DateTime>(60, timeForRetriever));

            //and an unspecified base energy
            int unsavedBaseEnergy = 0;
            _userRetriever.Setup(s => s.GetCurrentBaseEnergy(userId)).Returns(() => unsavedBaseEnergy);
            _userSaver.Setup(s => s.UpdateCurrentBaseEnergy(userId, 100)).Callback<Guid, int>((passedInUserId, passedInEnergy) => unsavedBaseEnergy = passedInEnergy);

            //should return 100, because maximum cannot be exceeded
            Assert.AreEqual(100, _userEnergyProvider.GetUserEnergy(userId, timeForCalculation));
        }

        [TestMethod]
        public void should_take_into_consideration_equipped_items_when_determining_max_energy()
        {
            Guid userId = Guid.NewGuid();

            //given a long period of time
            DateTime timeForRetriever = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            DateTime timeForCalculation = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 59);
            timeForCalculation = timeForCalculation.AddMinutes(300);

            //and a user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true).Verifiable();

            //with a last saved energy close to the maximum
            _userRetriever.Setup(s => s.GetLastSavedEnergy(userId))
                          .Returns(new UndeadEarth.Contract.Tuple<int, DateTime>(60, timeForRetriever));

            
            //and the following equipment, one of which is one time, one of which is not
            List<IItem> items = new List<IItem>();

            Mock<IItem> item = new Mock<IItem>();
            item.SetupGet(s => s.Energy).Returns(20);
            item.SetupGet(s => s.IsOneTimeUse).Returns(true);
            items.Add(item.Object);

            item = new Mock<IItem>();
            item.SetupGet(s => s.Energy).Returns(10);
            items.Add(item.Object);
            _userItemRetriever.Setup(s => s.GetUserItems(userId)).Returns(items);

            //and unspecified base energy
            int unsavedBaseEnergy = 0;
            _userRetriever.Setup(s => s.GetCurrentBaseEnergy(userId)).Returns(() => unsavedBaseEnergy);
            _userSaver.Setup(s => s.UpdateCurrentBaseEnergy(userId, 100)).Callback<Guid, int>((passedInUserId, passedInEnergy) => unsavedBaseEnergy = passedInEnergy);

            //should return the base energy pluse the combined energy of all equipment excluding one time use
            Assert.AreEqual(100 + 10, _userEnergyProvider.GetUserEnergy(userId, timeForCalculation));
        }
    }
}
