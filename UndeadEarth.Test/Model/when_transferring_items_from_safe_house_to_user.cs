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
    public class when_transferring_items_from_safe_house_to_user
    {
        private ISafeHouseDirector _safeHouseDirector;
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IItemRetriever> _itemRetriever;
        private Mock<ISafeHouseRetriever> _safeHouseRetriever;
        private Mock<IUserItemRetriever> _userItemRetriever;
        private Mock<ISafeHouseItemSaver> _safeHouseItemSaver;
        private Mock<IUserItemSaver> _userItemSaver;

        public when_transferring_items_from_safe_house_to_user()
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

            _safeHouseDirector.TransferItemFromSafeHouseToUser(userId, Guid.NewGuid(), Guid.NewGuid());
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

            _safeHouseDirector.TransferItemFromSafeHouseToUser(userId, safeHouseId, Guid.NewGuid());
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

            _safeHouseDirector.TransferItemFromSafeHouseToUser(userId, safeHouseId, itemId);
            _userRetriever.Verify();
            _safeHouseRetriever.Verify();
            _itemRetriever.Verify();

            _safeHouseRetriever.Verify(c => c.GetSafeHouseItemId(itemId, userId), Times.Never());
            _userItemSaver.Verify(c => c.SaveUserItem(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
            _safeHouseItemSaver.Verify(c => c.RemoveSafeHouseItem(It.IsAny<Guid>()), Times.Never());
        }

        [TestMethod]
        public void should_disregard_if_user_has_more_than_the_allowed_amount_of_items()
        {
            Guid userId = Guid.NewGuid();
            Guid safeHouseId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            Mock<IItem> item1 = new Mock<IItem>();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true).Verifiable();
            _safeHouseRetriever.Setup(c => c.SafeHouseExists(safeHouseId)).Returns(true).Verifiable();
            _itemRetriever.Setup(c => c.ItemExists(itemId)).Returns(true).Verifiable();
            _userItemRetriever.Setup(c => c.GetUserItemCount(userId)).Returns(20).Verifiable();

            _safeHouseDirector.TransferItemFromSafeHouseToUser(userId, safeHouseId, itemId);
            _userRetriever.Verify();
            _safeHouseRetriever.Verify();
            _itemRetriever.Verify();
            _userItemRetriever.Verify();

            _safeHouseRetriever.Verify(c => c.GetSafeHouseItemId(itemId, userId), Times.Never());
            _userItemSaver.Verify(c => c.SaveUserItem(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
            _safeHouseItemSaver.Verify(c => c.RemoveSafeHouseItem(It.IsAny<Guid>()), Times.Never());
        }

        [TestMethod]
        public void should_disregard_if_safe_house_does_not_have_item()
        {
            Guid userId = Guid.NewGuid();
            Guid safeHouseId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true).Verifiable();
            _safeHouseRetriever.Setup(c => c.SafeHouseExists(safeHouseId)).Returns(true).Verifiable();
            _itemRetriever.Setup(c => c.ItemExists(itemId)).Returns(true).Verifiable();
            _userItemRetriever.Setup(c => c.GetUserItemCount(userId)).Returns(1).Verifiable();
            _safeHouseRetriever.Setup(c => c.SafeHouseHasItem(itemId, userId)).Returns(false).Verifiable();
            _userRetriever.Setup(c => c.GetCurrentBaseSlots(userId)).Returns(9);

            _safeHouseDirector.TransferItemFromSafeHouseToUser(userId, safeHouseId, itemId);
            _userRetriever.Verify();
            _safeHouseRetriever.Verify();
            _itemRetriever.Verify();
            _userItemRetriever.Verify();

            _safeHouseRetriever.Verify(c => c.GetSafeHouseItemId(itemId, userId), Times.Never());
            _userItemSaver.Verify(c => c.SaveUserItem(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
            _safeHouseItemSaver.Verify(c => c.RemoveSafeHouseItem(It.IsAny<Guid>()), Times.Never());
        } 

        [TestMethod]
        public void should_save_item_in_user_inventory_and_take_out_of_safe_house_inventory()
        {
            Guid userId = Guid.NewGuid();
            Guid safeHouseId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();
            Guid safeHouseUserItemId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
            _safeHouseRetriever.Setup(c => c.SafeHouseExists(safeHouseId)).Returns(true);
            _itemRetriever.Setup(c => c.ItemExists(itemId)).Returns(true);
            _userItemRetriever.Setup(c => c.GetUserItemCount(userId)).Returns(1);
            _safeHouseRetriever.Setup(c => c.SafeHouseHasItem(itemId, userId)).Returns(true);
            _safeHouseRetriever.Setup(c => c.GetSafeHouseItemId(itemId, userId)).Returns(safeHouseUserItemId).Verifiable();
            _userRetriever.Setup(c => c.GetCurrentBaseSlots(userId)).Returns(9);
            _safeHouseDirector.TransferItemFromSafeHouseToUser(userId, safeHouseId, itemId);

            _userItemSaver.Verify(c => c.SaveUserItem(userId, itemId), Times.Once());
            _safeHouseItemSaver.Verify(c => c.RemoveSafeHouseItem(safeHouseUserItemId), Times.Once());
            
        }

    }
}