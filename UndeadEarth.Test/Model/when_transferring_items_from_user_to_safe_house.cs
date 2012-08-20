using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using Moq;
using UndeadEarth.Model;

namespace UndeadEarth.Test.Model
{
    [TestClass]
    public class when_transferring_items_from_user_to_safe_house
    {
        private ISafeHouseDirector _safeHouseDirector;
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IItemRetriever> _itemRetriever;
        private Mock<ISafeHouseRetriever> _safeHouseRetriever;
        private Mock<IUserItemRetriever> _userItemRetriever;
        private Mock<ISafeHouseItemSaver> _safeHouseItemSaver;
        private Mock<IUserItemSaver> _userItemSaver;

        public when_transferring_items_from_user_to_safe_house()
        {
            _userItemRetriever = new Mock<IUserItemRetriever>();
            _userRetriever = new Mock<IUserRetriever>();
            _itemRetriever = new Mock<IItemRetriever>();
            _safeHouseRetriever = new Mock<ISafeHouseRetriever>();
            _userItemRetriever = new Mock<IUserItemRetriever>();
            _safeHouseItemSaver = new Mock<ISafeHouseItemSaver>();
            _userItemSaver = new Mock<IUserItemSaver>();

            _safeHouseDirector = new SafeHouseDirector(_userRetriever.Object,
                                                       _itemRetriever.Object,
                                                       _safeHouseRetriever.Object,
                                                       _userItemRetriever.Object,
                                                       _safeHouseItemSaver.Object,
                                                       _userItemSaver.Object);
        }

        [TestMethod]
        public void should_disregard_if_user_does_not_exist()
        {
            Guid userId = Guid.NewGuid();
            Guid safeHouseId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(false).Verifiable();

            _safeHouseDirector.TransferItemFromUserToSafeHouse(userId, Guid.NewGuid(), Guid.NewGuid());
            _userRetriever.Verify();

            _safeHouseRetriever.Verify(c => c.GetSafeHouseItemId(itemId, userId), Times.Never());
            _userItemSaver.Verify(c => c.SaveUserItem(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
            _safeHouseItemSaver.Verify(c => c.RemoveSafeHouseItem(It.IsAny<Guid>()), Times.Never());
        }

        [TestMethod]
        public void should_disregard_if_safe_house_does_not_exist()
        {
            Guid userId = Guid.NewGuid();
            Guid safeHouseId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true).Verifiable();
            _safeHouseRetriever.Setup(c => c.SafeHouseExists(safeHouseId)).Returns(false).Verifiable();

            _safeHouseDirector.TransferItemFromUserToSafeHouse(userId, safeHouseId, Guid.NewGuid());
            _userRetriever.Verify();
            _safeHouseRetriever.Verify();

            _safeHouseRetriever.Verify(c => c.GetSafeHouseItemId(itemId, userId), Times.Never());
            _userItemSaver.Verify(c => c.SaveUserItem(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
            _safeHouseItemSaver.Verify(c => c.RemoveSafeHouseItem(It.IsAny<Guid>()), Times.Never());
        }

        [TestMethod]
        public void should_disregard_if_item_does_not_exist()
        {
            Guid userId = Guid.NewGuid();
            Guid safeHouseId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true).Verifiable();
            _safeHouseRetriever.Setup(c => c.SafeHouseExists(safeHouseId)).Returns(true).Verifiable();
            _itemRetriever.Setup(c => c.ItemExists(itemId)).Returns(false).Verifiable();

            _safeHouseDirector.TransferItemFromUserToSafeHouse(userId, safeHouseId, itemId);
            _userRetriever.Verify();
            _safeHouseRetriever.Verify();
            _itemRetriever.Verify();

            _safeHouseRetriever.Verify(c => c.GetSafeHouseItemId(itemId, userId), Times.Never());
            _userItemSaver.Verify(c => c.SaveUserItem(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
            _safeHouseItemSaver.Verify(c => c.RemoveSafeHouseItem(It.IsAny<Guid>()), Times.Never());
        }

        [TestMethod]
        public void should_disregard_if_user_does_not_have_item()
        {
            Guid userId = Guid.NewGuid();
            Guid safeHouseId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
            _safeHouseRetriever.Setup(c => c.SafeHouseExists(safeHouseId)).Returns(true);
            _itemRetriever.Setup(c => c.ItemExists(itemId)).Returns(true);
            _userItemRetriever.Setup(c => c.UserHasItem(userId, itemId)).Returns(false).Verifiable();

            _safeHouseDirector.TransferItemFromUserToSafeHouse(userId, safeHouseId, itemId);
            _userItemRetriever.Verify();

            _safeHouseRetriever.Verify(c => c.GetSafeHouseItemId(itemId, userId), Times.Never());
            _userItemSaver.Verify(c => c.SaveUserItem(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
            _safeHouseItemSaver.Verify(c => c.RemoveSafeHouseItem(It.IsAny<Guid>()), Times.Never());
        }

        [TestMethod]
        public void should_save_item_in_safe_house_inventory_and_remove_from_user_inventory()
        {
            Guid userId = Guid.NewGuid();
            Guid safeHouseId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();
            Guid userItemId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
            _safeHouseRetriever.Setup(c => c.SafeHouseExists(safeHouseId)).Returns(true);
            _itemRetriever.Setup(c => c.ItemExists(itemId)).Returns(true);
            _userItemRetriever.Setup(c => c.UserHasItem(userId, itemId)).Returns(true);
            _userItemRetriever.Setup(c => c.GetUserItemId(userId, itemId)).Returns(userItemId).Verifiable();

            _safeHouseDirector.TransferItemFromUserToSafeHouse(userId, safeHouseId, itemId);

            _safeHouseItemSaver.Verify(c => c.SaveItemInSafeHouse(safeHouseId, itemId, userId), Times.Once());
            _userItemSaver.Verify(c => c.RemoveUserItem(userItemId), Times.Once());
            

        }
    }
}
