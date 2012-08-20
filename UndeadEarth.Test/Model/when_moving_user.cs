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
    public class when_moving_user
    {
        private IUserMoveDirector _userMoveDirector;
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IUserSaver> _userSaver;
        private Mock<IUserEnergyProvider> _userEnergyProvider;
        private Mock<IDistanceCalculator> _distanceCalculator;
        private Mock<IUserCountsSaver> _userCountsSaver;
        public when_moving_user()
        {
            _userRetriever = new Mock<IUserRetriever>();
            _userSaver = new Mock<IUserSaver>();
            _userEnergyProvider = new Mock<IUserEnergyProvider>();
            _distanceCalculator = new Mock<IDistanceCalculator>();
            _userCountsSaver = new Mock<IUserCountsSaver>();
            _userMoveDirector = new UserMoveDirector(_userRetriever.Object,
                _userEnergyProvider.Object, 
                _userSaver.Object, 
                _distanceCalculator.Object,
                _userCountsSaver.Object);
        }

        [TestMethod]
        public void should_disregard_move_if_user_does_not_exist()
        {
            Guid userId = Guid.NewGuid();
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(false).Verifiable();

            _userMoveDirector.MoveUser(userId, 0, 0);
            _userRetriever.Verify();
            _userSaver.Verify(s => s.UpdateUserLocation(It.IsAny<Guid>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never());
        }

        [TestMethod]
        public void should_perform_partial_move_if_user_doesnt_have_enough_energy_to_go_the_entire_length()
        {
            Guid userId = Guid.NewGuid();
            Mock<IUser> mockUserToReturn = new Mock<IUser>();
            
            mockUserToReturn.SetupGet(s => s.Latitude).Returns(10);
            mockUserToReturn.SetupGet(s => s.Longitude).Returns(10);

            _distanceCalculator.Setup(s => s.CalculateMiles(10, 10, 30, 30)).Returns(20);

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _userRetriever.Setup(s => s.GetUserById(userId)).Returns(mockUserToReturn.Object);
            _userEnergyProvider.Setup(s => s.GetUserEnergy(userId, It.IsAny<DateTime>())).Returns(10);

            _userMoveDirector.MoveUser(userId, 30, 30);
            _userSaver.Verify(s => s.UpdateUserLocation(userId, 20, 20));
        }

        [TestMethod]
        public void should_move_user_and_update_users_location_if_user_has_enough_energy_to_move_to_location()
        {
            Guid userId = Guid.NewGuid();
            Mock<IUser> mockUserToReturn = new Mock<IUser>();
            //setup users location
            mockUserToReturn.SetupGet(s => s.Latitude).Returns(1);
            mockUserToReturn.SetupGet(s => s.Longitude).Returns(10);

            //setup distance to return a distance of 10
            _distanceCalculator.Setup(s => s.CalculateMiles(1, 10, 35, 25)).Returns(10);

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _userRetriever.Setup(s => s.GetUserById(userId)).Returns(mockUserToReturn.Object);

            //user has 20 energy whch means user can mover there
            _userEnergyProvider.Setup(s => s.GetUserEnergy(userId, It.IsAny<DateTime>())).Returns(20);

            _userMoveDirector.MoveUser(userId, 35, 25);
            _userSaver.Verify(s => s.UpdateUserLocation(userId, 35, 25), Times.Exactly(1));
        }

        [TestMethod]
        public void should_update_users_energy_based_on_distance_moved()
        {
            Guid userId = Guid.NewGuid();
            Mock<IUser> mockUserToReturn = new Mock<IUser>();
            //setup users location
            mockUserToReturn.SetupGet(s => s.Latitude).Returns(1);
            mockUserToReturn.SetupGet(s => s.Longitude).Returns(10);

            //setup distance to return a distance of 15
            _distanceCalculator.Setup(s => s.CalculateMiles(1, 10, 35, 25)).Returns(15);

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _userRetriever.Setup(s => s.GetUserById(userId)).Returns(mockUserToReturn.Object);

            //user has 20 energy whch means user can mover there
            _userEnergyProvider.Setup(s => s.GetUserEnergy(userId, It.IsInRange<DateTime>(DateTime.Today, DateTime.Today.AddDays(1), Range.Inclusive))).Returns(20);

            _userMoveDirector.MoveUser(userId, 35, 25);

            //new energy saved to the database should be 20-15 (5)
            _userSaver.Verify(s => s.SaveLastEnergy(userId, 5, It.IsInRange<DateTime>(DateTime.Today, DateTime.Today.AddDays(1), Range.Inclusive)), Times.Exactly(1));
        }

        [TestMethod]
        public void should_record_how_many_miles_have_been_moved_for_achievements()
        {
            Guid userId = Guid.NewGuid();
            Mock<IUser> mockUserToReturn = new Mock<IUser>();
            //setup users location
            mockUserToReturn.SetupGet(s => s.Latitude).Returns(1);
            mockUserToReturn.SetupGet(s => s.Longitude).Returns(10);

            //setup distance to return a distance of 15
            _distanceCalculator.Setup(s => s.CalculateMiles(1, 10, 35, 25)).Returns(15);

            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _userRetriever.Setup(s => s.GetUserById(userId)).Returns(mockUserToReturn.Object);

            //user has 20 energy whch means user can mover there
            _userEnergyProvider.Setup(s => s.GetUserEnergy(userId, It.IsInRange<DateTime>(DateTime.Today, DateTime.Today.AddDays(1), Range.Inclusive))).Returns(20);

            _userMoveDirector.MoveUser(userId, 35, 25);

            _userCountsSaver.Verify(s => s.AddMiles(userId, 15));
        }
    }
}
