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
    public class when_buying_items_from_shop
    {
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IItemRetriever> _itemRetriever;
        private Mock<IStoreRetriever> _storeRetriever;
        private Mock<IUserItemRetriever> _userItemRetriever;
        private Mock<IUserSaver> _userSaver;
        private Mock<IUserItemSaver> _userItemSaver;

        private IShopDirector _shopDirector;

        public when_buying_items_from_shop()
        {
            _userRetriever = new Mock<IUserRetriever>();
            _itemRetriever = new Mock<IItemRetriever>();
            _storeRetriever = new Mock<IStoreRetriever>();
            _userItemRetriever = new Mock<IUserItemRetriever>();
            _userSaver = new Mock<IUserSaver>();
            _userItemSaver = new Mock<IUserItemSaver>();

            _shopDirector = new ShopDirector(_userRetriever.Object
                                           , _itemRetriever.Object
                                           , _storeRetriever.Object
                                           , _userItemRetriever.Object
                                           , _userSaver.Object
                                           , _userItemSaver.Object);
        }

        [TestMethod]
        public void should_disregard_if_user_does_not_exist()
        {
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(false).Verifiable();
            _shopDirector.BuyItem(userId, itemId);

            _userRetriever.Verify();
            _userSaver.Verify(c => c.AddMoney(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
            _userItemSaver.Verify(c => c.SaveUserItem(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
        }

        [TestMethod]
        public void should_disregard_if_item_does_not_exist()
        {
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            IItem item = null;

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
            _itemRetriever.Setup(c => c.GetItemById(itemId)).Returns(item).Verifiable();

            _shopDirector.BuyItem(userId, itemId);

            _itemRetriever.Verify();
            _userSaver.Verify(c => c.AddMoney(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
            _userItemSaver.Verify(c => c.SaveUserItem(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
        }

        [TestMethod]
        public void should_disregard_if_user_does_not_have_enough_money()
        {
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();
            Mock<IItem> item = new Mock<IItem>();
            item.SetupGet(c => c.Id).Returns(itemId);
            item.SetupGet(c => c.Price).Returns(100);

            Mock<IUser> user = new Mock<IUser>();
            user.SetupGet(c => c.Id).Returns(userId);
            user.SetupGet(c => c.Money).Returns(50);

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
            _itemRetriever.Setup(c => c.GetItemById(itemId)).Returns(item.Object);
            _userRetriever.Setup(c => c.GetUserById(userId)).Returns(user.Object).Verifiable();

            _shopDirector.BuyItem(userId, itemId);

            _userRetriever.Verify();
            _userSaver.Verify(c => c.AddMoney(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
            _userItemSaver.Verify(c => c.SaveUserItem(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
        }

        [TestMethod]
        public void should_disregard_if_store_does_not_exist()
        {
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();
            Mock<IItem> item = new Mock<IItem>();
            item.SetupGet(c => c.Id).Returns(itemId);
            item.SetupGet(c => c.Price).Returns(100);

            Mock<IUser> user = new Mock<IUser>();
            user.SetupGet(c => c.Id).Returns(userId);
            user.SetupGet(c => c.Money).Returns(150);
            user.SetupGet(c => c.Longitude).Returns((double)10.0);
            user.SetupGet(c => c.Latitude).Returns((double)10.0);

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
            _itemRetriever.Setup(c => c.GetItemById(itemId)).Returns(item.Object);
            _userRetriever.Setup(c => c.GetUserById(userId)).Returns(user.Object);
            _storeRetriever.Setup(c => c.StoreExists(user.Object.Latitude, user.Object.Longitude)).Returns(false).Verifiable();

            _shopDirector.BuyItem(userId, itemId);

            _storeRetriever.Verify();
            _userSaver.Verify(c => c.AddMoney(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
            _userItemSaver.Verify(c => c.SaveUserItem(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
        }

        [TestMethod]
        public void should_disregard_if_user_has_max_items_already()
        {
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();
            Mock<IItem> item = new Mock<IItem>();
            item.SetupGet(c => c.Id).Returns(itemId);
            item.SetupGet(c => c.Price).Returns(100);

            Mock<IUser> user = new Mock<IUser>();
            user.SetupGet(c => c.Id).Returns(userId);
            user.SetupGet(c => c.Money).Returns(150);
            user.SetupGet(c => c.Longitude).Returns((double)10.0);
            user.SetupGet(c => c.Latitude).Returns((double)10.0);

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
            _itemRetriever.Setup(c => c.GetItemById(itemId)).Returns(item.Object);
            _userRetriever.Setup(c => c.GetUserById(userId)).Returns(user.Object);
            _storeRetriever.Setup(c => c.StoreExists(user.Object.Latitude, user.Object.Longitude)).Returns(true);
            _userItemRetriever.Setup(c => c.GetUserItemCount(userId)).Returns(9).Verifiable();

            _shopDirector.BuyItem(userId, itemId);

            _userItemRetriever.Verify();
            _userSaver.Verify(c => c.AddMoney(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never());
            _userItemSaver.Verify(c => c.SaveUserItem(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
        }

        [TestMethod]
        public void should_take_money_from_user_and_add_item_to_user_inventory()
        {
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();
            Mock<IItem> item = new Mock<IItem>();
            item.SetupGet(c => c.Id).Returns(itemId);
            item.SetupGet(c => c.Price).Returns(100);

            Mock<IUser> user = new Mock<IUser>();
            user.SetupGet(c => c.Id).Returns(userId);
            user.SetupGet(c => c.Money).Returns(150);
            user.SetupGet(c => c.Longitude).Returns((double)10.0);
            user.SetupGet(c => c.Latitude).Returns((double)10.0);

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
            _itemRetriever.Setup(c => c.GetItemById(itemId)).Returns(item.Object);
            _userRetriever.Setup(c => c.GetUserById(userId)).Returns(user.Object);
            _storeRetriever.Setup(c => c.StoreExists(user.Object.Latitude, user.Object.Longitude)).Returns(true);
            _userItemRetriever.Setup(c => c.GetUserItemCount(userId)).Returns(0);
            _userRetriever.Setup(c => c.GetCurrentBaseSlots(userId)).Returns(9);

            _shopDirector.BuyItem(userId, itemId);

            _userSaver.Verify(c => c.AddMoney(userId, (-1 * item.Object.Price)), Times.Once());
            _userItemSaver.Verify(c => c.SaveUserItem(userId, itemId), Times.Once());
        }


    }
}
