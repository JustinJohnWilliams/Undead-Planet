using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UndeadEarth.Contract;
using UndeadEarth.Model;

namespace UndeadEarth.Test.Model
{
    [TestClass]
    public class when_determining_users_max_energy
    {
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IUserSaver> _userSaver;
        private Mock<IUserItemRetriever> _userItemRetriever;
        private IUserEnergyProvider _userEnergyProvider;
        public when_determining_users_max_energy()
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
            
            //given non existant user
            _userRetriever.Setup( s=> s.UserExists(userId)).Returns(false);

            //should return max eneryg of zero
            int energy = _userEnergyProvider.GetUserMaxEnergy(userId);
            Assert.AreEqual(0, energy);
        }

        [TestMethod]
        public void should_return_users_base_energy_if_user_does_not_have_any_items()
        {
            Guid userId = Guid.NewGuid();

            //given user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);

            //and a base energy of 100
            _userRetriever.Setup(s => s.GetCurrentBaseEnergy(userId)).Returns(100);

            //and user with no inventory
            _userItemRetriever.Setup(s => s.GetUserItems(userId)).Returns(() => null);

            //should set user's energy to 100
            int energy = _userEnergyProvider.GetUserMaxEnergy(userId);
            Assert.AreEqual(100, energy);
        }

        [TestMethod]
        public void should_return_base_eneryg_plus_any_equiped_items_that_affect_energy()
        {
            Guid userId = Guid.NewGuid();

            //given user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);

            //and a base energy of 100
            _userRetriever.Setup(s => s.GetCurrentBaseEnergy(userId)).Returns(100);

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

            //should return 110 for base energy which excludes one time use
            int energy = _userEnergyProvider.GetUserMaxEnergy(userId);
            Assert.AreEqual(110, energy);
        }
    }
}
