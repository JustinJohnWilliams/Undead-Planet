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
    public class when_creating_user
    {
        public class HotZone : IHotZone
        {
            public Guid Id { get; set; }

            public double Latitude { get; set; }

            public double Longitude { get; set; }

            public string Name { get; set; }
        }

        private Mock<IUserCountsSaver> _userCountsSaver;
        private IUserCreationService _userCreationService;
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IUserSaver> _userSaver;
        private Mock<IUserItemSaver> _userItemSaver;
        private Mock<ISafeHouseItemSaver> _safeHouseItemSaver;
        private Mock<IHotZoneRetriever> _hotZoneRetriever;

        private long _facebookUserId;
        private Guid _startingHotZoneId;
        private int _attackPower;
        private int _energy;
        private string _name;

        [TestInitialize]
        public void TestInitalize()
        {
            _userSaver = new Mock<IUserSaver>();
            _userRetriever = new Mock<IUserRetriever>();
            _safeHouseItemSaver = new Mock<ISafeHouseItemSaver>();
            _userItemSaver = new Mock<IUserItemSaver>();
            _hotZoneRetriever = new Mock<IHotZoneRetriever>();
            _userCountsSaver = new Mock<IUserCountsSaver>();

            _userCreationService = new UserCreationService(_userRetriever.Object, _userSaver.Object, _userItemSaver.Object, _safeHouseItemSaver.Object, _hotZoneRetriever.Object, _userCountsSaver.Object);

            _startingHotZoneId = Guid.NewGuid();
            _facebookUserId = 100;
            _attackPower = 1;
            _energy = 100;
            _name = "name";

            _hotZoneRetriever.Setup(s => s.IsStartingHotZone(_startingHotZoneId)).Returns(true);
            _hotZoneRetriever.Setup(s => s.GetHotZone(It.IsAny<Guid>())).Returns(new HotZone());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _userRetriever.Verify();
            _userSaver.Verify();
            _safeHouseItemSaver.Verify();
            _hotZoneRetriever.Verify();
        }

        [TestMethod]
        public void should_disregard_creation_if_user_already_exists_for_facebookId()
        {
            GivenFacebookUserAlreadyExists();
            TheUser();
            ShouldNotHaveBeenCreated();
        }

        [TestMethod]
        public void should_create_user_with_starting_location()
        {
            TheUser();
            ShouldHaveBeenCreated();
        }

        [TestMethod]
        public void should_start_with_5_tents_on_person()
        {
            TheUser();
            ShouldHaveFiveTents();
        }

        [TestMethod]
        public void should_start_with_attack_power_of_1()
        {
            TheUser();
            ShouldHaveAttackPowerOf(1);
        }

        [TestMethod]
        public void should_start_with_energy_of_100()
        {
            TheUser();
            ShouldHaveEnergyOf(100);
        }

        [TestMethod]
        public void should_disregard_creation_if_location_specified_is_not_a_starting_hot_zone()
        {
            GivenTheLocationIsNotAHotZone();
            TheUser();
            ShouldNotHaveBeenCreated();
        }

        [TestMethod]
        public void should_set_latitude_longitude_position_for_hotzone()
        {
            GivenTheHotZoneLatLongIs(93, 22);
            TheUser();
            ShouldHaveStartingLocationOf(93, 22);
        }

        [TestMethod]
        public void should_insert_user_counts()
        {
            TheUser();
            ShouldHaveUserCounts();
        }

        private void ShouldHaveStartingLocationOf(double latitude, double longitude)
        {
            _userSaver.Verify(s => s.UpdateUserLocation(It.IsAny<Guid>(), latitude, longitude));
        }

        private void GivenTheLocationIsNotAHotZone()
        {
            _hotZoneRetriever.Setup(s => s.IsStartingHotZone(_startingHotZoneId)).Returns(false);
        }

        private void ShouldHaveEnergyOf(int energy)
        {
            _userSaver.Verify(s => s.InsertUser(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), energy));
        }


        private void ShouldHaveAttackPowerOf(int attackPower)
        {
            _userSaver.Verify(s => s.InsertUser(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<Guid>(), attackPower, It.IsAny<int>()));
        }

        private void ShouldHaveFiveTents()
        {
            _userItemSaver.Verify(s => s.AddTent(It.IsAny<Guid>(), 5));
        }

        private void ShouldHaveBeenCreated()
        {
            _userSaver.Verify(s => s.InsertUser(It.IsAny<Guid>(), _facebookUserId, _name, _startingHotZoneId, _attackPower, _energy), Times.Once());
        }

        private void ShouldNotHaveBeenCreated()
        {
            _userSaver.Verify(s => s.InsertUser(It.IsAny<Guid>(), _facebookUserId, _name, _startingHotZoneId, _attackPower, _energy), Times.Never());
        }

        private void GivenFacebookUserAlreadyExists()
        {
            _userRetriever.Setup(s => s.FacebookUserExists(_facebookUserId)).Returns(true).Verifiable();
        }

        private void GivenFacebookUserId(long facebookUserId)
        {
            _facebookUserId = 100;
        }

        private void TheUser()
        {
            _userCreationService.CreateUser(_facebookUserId, _name, _startingHotZoneId);
        }

        private void GivenTheHotZoneLatLongIs(double latitude, double longitude)
        {
            _hotZoneRetriever.Setup(s => s.GetHotZone(_startingHotZoneId)).Returns(new HotZone { Latitude = latitude, Longitude = longitude });
        }

        private void ShouldHaveUserCounts()
        {
            _userCountsSaver.Verify(s => s.InsertCounts(It.IsAny<Guid>()));
        }
    }
}
