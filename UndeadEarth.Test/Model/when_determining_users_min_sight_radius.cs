using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UndeadEarth.Contract;
using UndeadEarth.Model;

namespace UndeadEarth.Test.Model
{
    [TestClass]
    public class when_determining_users_min_sight_radius
    {
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IUserSaver> _userSaver;
        private Mock<IUserItemRetriever> _userItemRetriever;
        private IUserSightRadiusProvider _userSightRadiusProvider;
        public when_determining_users_min_sight_radius()
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

            //given non existant user
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(false);

            //should return min sight radius of zero
            int sightRadius = _userSightRadiusProvider.GetUserMinSightRadius(userId);
            Assert.AreEqual(0, sightRadius);
        }

        [TestMethod]
        public void should_return_users_base_sight_radius_if_user_does_not_have_any_items()
        {
            Guid userId = Guid.NewGuid();

            //given user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);

            //and a base sight radius
            _userRetriever.Setup(s => s.GetCurrentBaseSightRadius(userId)).Returns(5);

            //and user with no inventory
            _userItemRetriever.Setup(s => s.GetUserItems(userId)).Returns(() => null);

            //should set user's sight radius of to 5
            int sightRadius = _userSightRadiusProvider.GetUserMinSightRadius(userId);
            Assert.AreEqual(5, sightRadius);
        }

        [TestMethod]
        public void should_return_base_sight_radius_plus_any_equiped_items_that_affect_sight_radius()
        {
            Guid userId = Guid.NewGuid();

            //given user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);

            //and a base sight radius of 5
            _userRetriever.Setup(s => s.GetCurrentBaseSightRadius(userId)).Returns(5);

            //and the following equipment, one of which is one time, one of which is not
            List<IItem> items = new List<IItem>();

            Mock<IItem> item = new Mock<IItem>();
            item.SetupGet(s => s.Distance).Returns(20);
            item.SetupGet(s => s.IsOneTimeUse).Returns(true);
            items.Add(item.Object);

            item = new Mock<IItem>();
            item.SetupGet(s => s.Distance).Returns(10);
            items.Add(item.Object);
            _userItemRetriever.Setup(s => s.GetUserItems(userId)).Returns(items);

            //should return 15 for base sight radius which excludes one time use
            int energy = _userSightRadiusProvider.GetUserMinSightRadius(userId);
            Assert.AreEqual(5 + 10, energy);
        }
    }
}
