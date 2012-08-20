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
    public class when_retrieving_user_attack_power 
    {
        private IUserAttackPowerProvider _userAttackPowerProvider;
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IUserItemRetriever> _userItemRetriever;
        public when_retrieving_user_attack_power()
        {
            _userRetriever = new Mock<IUserRetriever>();
            _userItemRetriever = new Mock<IUserItemRetriever>();
            _userAttackPowerProvider = new UserAttackPowerProvider(_userRetriever.Object, _userItemRetriever.Object);
        }

        [TestMethod]
        public void should_return_zero_for_attack_power_if_user_does_not_exist()
        {
            Guid userId = Guid.NewGuid();
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(false).Verifiable();
            

            int attackPower = _userAttackPowerProvider.GetAttackPower(userId);
            Assert.AreEqual(0, attackPower);
            _userRetriever.Verify();
        }

        [TestMethod]
        public void should_return_one_for_attack_power_if_user_exists_but_does_not_have_any_inventory()
        {
            Guid userId = Guid.NewGuid();
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _userRetriever.Setup(s => s.GetCurrentBaseAttackPower(userId)).Returns(1);
            _userItemRetriever.Setup(s => s.GetUserItems(userId)).Returns(new List<IItem>());

            int attackPower = _userAttackPowerProvider.GetAttackPower(userId);
            Assert.AreEqual(1, attackPower);
        }

        [TestMethod]
        public void should_return_the_summation_of_the_attack_powers_of_items_on_person()
        {
            Guid userId = Guid.NewGuid();
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);
            _userRetriever.Setup(s => s.GetCurrentBaseAttackPower(userId)).Returns(1);

            List<IItem> items = new List<IItem>();
            
            Mock<IItem> item1 = new Mock<IItem>();
            item1.SetupGet(s => s.Attack).Returns(2);
            items.Add(item1.Object);

            Mock<IItem> item2 = new Mock<IItem>();
            item2.SetupGet(s => s.Attack).Returns(5);
            items.Add(item2.Object);

            Mock<IItem> item3 = new Mock<IItem>();
            item3.SetupGet(s => s.Attack).Returns(7);
            items.Add(item3.Object);

            _userItemRetriever.Setup(s => s.GetUserItems(userId)).Returns(items);

            int attackPower = _userAttackPowerProvider.GetAttackPower(userId);
            Assert.AreEqual(15, attackPower);
        }
    }
}
