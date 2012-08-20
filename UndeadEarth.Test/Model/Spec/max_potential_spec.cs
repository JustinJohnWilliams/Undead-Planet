using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using Moq;
using UndeadEarth.Model;

namespace UndeadEarth.Test.Model.Spec
{
    public static class max_potential_spec_extensions
    {
        public static List<IItem> AndItem(this List<IItem> items, IItem item)
        {
            items.Add(item);
            return items;
        }
    }

    [TestClass]
    public class max_potential_spec
    {
        protected Mock<IUserRetriever> _userRetriever;
        protected Mock<IItemRetriever> _itemRetriever;
        protected IUserPotentialProvider _userPotentialProvider;
        protected Mock<ISafeHouseRetriever> _safeHouseRetriever;
        protected Mock<IUserItemRetriever> _userItemRetriever;
        protected Guid _userId;
        protected List<IItem> _userItems;
        protected List<IItem> _safeHouseItems;
        protected List<IItem> _shopItems;

        [TestInitialize]
        public void TestInitialize()
        {
            _userRetriever = new Mock<IUserRetriever>();
            _itemRetriever = new Mock<IItemRetriever>();
            _safeHouseRetriever = new Mock<ISafeHouseRetriever>();
            _userItemRetriever = new Mock<IUserItemRetriever>();
            _userId = Guid.NewGuid();
            _userItems = new List<IItem>();
            _safeHouseItems = new List<IItem>();
            _shopItems = new List<IItem>();
            _userPotentialProvider = new UserPotentialProvider(_userRetriever.Object, _itemRetriever.Object, _safeHouseRetriever.Object, _userItemRetriever.Object);

            _userItemRetriever.Setup(s => s.GetUserItems(_userId)).Returns(_userItems);
            _safeHouseRetriever.Setup(s => s.GetItems(_userId)).Returns(_safeHouseItems);
            _itemRetriever.Setup(s => s.GetAllBelowPrice(It.IsAny<int>())).Returns(_shopItems);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _userRetriever.Verify();
            _itemRetriever.Verify();
            _safeHouseRetriever.Verify();
            _userItemRetriever.Verify();
        }

        public void GivenUserExists()
        {
            _userRetriever.Setup(s => s.UserExists(_userId)).Returns(true);
        }

        public void GivenBaseAttack(int attack)
        {
            _userRetriever.Setup(s => s.GetAttackPowerForDifficultyCalculation(_userId)).Returns(attack);
        }

        public void GivenMoney(int money)
        {
            _userRetriever.Setup(s => s.GetCurrentMoney(_userId)).Returns(money);
        }

        public void GivenBaseEnergy(int energy)
        {
            _userRetriever.Setup(s => s.GetEnergyForDifficultyCalculation(_userId)).Returns(energy);
        }

        public List<IItem> GivenShopItem(IItem item)
        {
            _shopItems.Add(item);
            return _shopItems;
        }

        public List<IItem> GivenSafeHouseItem(IItem item)
        {
            _safeHouseItems.Add(item);
            return _safeHouseItems;
        }

        public  List<IItem> GivenUserItem(IItem item)
        {
            _userItems.Add(item);
            return _userItems;
        }

        public void GivenAvailableItemSlots(int count)
        {
            int doubledItemsIncludingOneTime = count * 2;
            doubledItemsIncludingOneTime = Convert.ToInt32((double)doubledItemsIncludingOneTime + (double)doubledItemsIncludingOneTime * .2);

            _userRetriever.Setup(s => s.GetCurrentBaseSlots(_userId)).Returns(doubledItemsIncludingOneTime);
        }
    }
}
