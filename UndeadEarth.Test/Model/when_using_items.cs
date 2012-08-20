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
    public class when_using_items
    {
        private Mock<IUserRetriever> _userRetriever;
        private Mock<IItemRetriever> _itemRetriever;
        private Mock<IUserItemSaver> _userItemSaver;
        private Mock<IUserItemRetriever> _userItemRetriever;
        private Mock<IUserSaver> _userSaver;
        private Mock<IUserEnergyProvider> _userEnergyProvider;
        private Mock<IUserSightRadiusProvider> _userSightRadiusProvider;

        private IItemUsageDirector _itemUsageDirector;

        public when_using_items()
        {
            _userRetriever = new Mock<IUserRetriever>();
            _itemRetriever = new Mock<IItemRetriever>();
            _userItemSaver = new Mock<IUserItemSaver>();
            _userItemRetriever = new Mock<IUserItemRetriever>();
            _userSaver = new Mock<IUserSaver>();
            _userEnergyProvider = new Mock<IUserEnergyProvider>();
            _userSightRadiusProvider = new Mock<IUserSightRadiusProvider>();

            _itemUsageDirector = new ItemUsageDirector(_userRetriever.Object, _itemRetriever.Object
                                                     , _userItemSaver.Object, _userItemRetriever.Object
                                                     , _userSaver.Object, _userEnergyProvider.Object
                                                     , _userSightRadiusProvider.Object);
        }

        [TestMethod]
        public void should_return_if_user_does_not_exist()
        {
            Guid userId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(false).Verifiable();

            _itemUsageDirector.UseItem(userId, Guid.NewGuid());

            _userRetriever.Verify();
            _userItemSaver.Verify(c => c.RemoveUserItem(It.IsAny<Guid>()), Times.Never());
            _userSaver.Verify(c => c.SaveLastEnergy(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never());
        }

        [TestMethod]
        public void should_return_if_item_does_not_exist()
        {
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
            _itemRetriever.Setup(c => c.ItemExists(itemId)).Returns(false).Verifiable();

            _itemUsageDirector.UseItem(userId, itemId);

            _itemRetriever.Verify();
            _userItemSaver.Verify(c => c.RemoveUserItem(It.IsAny<Guid>()), Times.Never());
            _userSaver.Verify(c => c.SaveLastEnergy(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never());
        }

        [TestMethod]
        public void should_return_if_item_is_not_one_time_use_item()
        {
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            Mock<IItem> item = new Mock<IItem>();
            item.SetupGet(c => c.Id).Returns(itemId);
            item.SetupGet(c => c.IsOneTimeUse).Returns(false);

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);
            _itemRetriever.Setup(c => c.ItemExists(itemId)).Returns(true);
            _itemRetriever.Setup(c => c.GetItemById(itemId)).Returns(item.Object).Verifiable();

            _itemUsageDirector.UseItem(userId, itemId);

            _itemRetriever.Verify();
            _userItemSaver.Verify(c => c.RemoveUserItem(It.IsAny<Guid>()), Times.Never());
            _userSaver.Verify(c => c.SaveLastEnergy(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never());
        }

        [TestMethod]
        public void should_change_users_energy_if_item_has_energy_attribute_not_equal_to_zero()
        {
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);

            Mock<IItem> mockItem = new Mock<IItem>();
            mockItem.SetupGet(c => c.Energy).Returns(10);
            mockItem.SetupGet(c => c.IsOneTimeUse).Returns(true);

            _itemRetriever.Setup(c => c.ItemExists(itemId)).Returns(true);
            _itemRetriever.Setup(c => c.GetItemById(itemId)).Returns(mockItem.Object);

            _userEnergyProvider.Setup(c => c.GetUserEnergy(userId, It.IsAny<DateTime>())).Returns(10);
            _userEnergyProvider.Setup(c => c.GetUserMaxEnergy(userId)).Returns(100);

            _itemUsageDirector.UseItem(userId, itemId);

            _userSaver.Verify(c => c.SaveLastEnergy(userId, 20, It.IsAny<DateTime>()));
        }

        [TestMethod]
        public void should_set_to_max_energy_if_item_energy_is_greater_than_max()
        {
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);

            Mock<IItem> mockItem = new Mock<IItem>();
            mockItem.SetupGet(c => c.Energy).Returns(20);
            mockItem.SetupGet(c => c.IsOneTimeUse).Returns(true);

            _itemRetriever.Setup(c => c.ItemExists(itemId)).Returns(true);
            _itemRetriever.Setup(c => c.GetItemById(itemId)).Returns(mockItem.Object);

            _userEnergyProvider.Setup(c => c.GetUserEnergy(userId, It.IsAny<DateTime>())).Returns(90);
            _userEnergyProvider.Setup(c => c.GetUserMaxEnergy(userId)).Returns(100);

            _itemUsageDirector.UseItem(userId, itemId);

            _userSaver.Verify(c => c.SaveLastEnergy(userId, 100, It.IsAny<DateTime>()));
        }

        [TestMethod]
        public void should_set_energy_to_zero_if_item_energy_plus_user_energy_is_negative()
        {
            Guid userId = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            _userRetriever.Setup(c => c.UserExists(userId)).Returns(true);

            Mock<IItem> mockItem = new Mock<IItem>();
            mockItem.SetupGet(c => c.Energy).Returns(10);
            mockItem.SetupGet(c => c.IsOneTimeUse).Returns(true);

            _itemRetriever.Setup(c => c.ItemExists(itemId)).Returns(true);
            _itemRetriever.Setup(c => c.GetItemById(itemId)).Returns(mockItem.Object);

            _userEnergyProvider.Setup(c => c.GetUserEnergy(userId, It.IsAny<DateTime>())).Returns(-30);
            _userEnergyProvider.Setup(c => c.GetUserMaxEnergy(userId)).Returns(100);

            _itemUsageDirector.UseItem(userId, itemId);

            _userSaver.Verify(c => c.SaveLastEnergy(userId, 0, It.IsAny<DateTime>()));
        }

        [TestMethod]
        public void should_change_users_sight_radius_if_item_has_a_distance_attribute_not_equal_to_zero()
        {
            Guid userId = Guid.NewGuid();

            Guid itemId = Guid.NewGuid();

            //given a user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);

            //and an item that exists
            Mock<IItem> mockItem = new Mock<IItem>();
            mockItem.SetupGet(s => s.Distance).Returns(15);
            mockItem.SetupGet(s => s.IsOneTimeUse).Returns(true);

            _itemRetriever.Setup(s => s.ItemExists(itemId)).Returns(true);
            
            _itemRetriever.Setup(s => s.GetItemById(itemId)).Returns(mockItem.Object);

            //and a user sight radius
            _userSightRadiusProvider.Setup(s => s.GetUserSightRadius(userId, It.IsAny<DateTime>())).Returns(15);
            
            //and a min sight radius
            _userSightRadiusProvider.Setup(s => s.GetUserMinSightRadius(userId)).Returns(10);

            //should update user's sight radius to include attributes of item
            _itemUsageDirector.UseItem(userId, itemId);

            _userSaver.Verify(s => s.SaveLastSightRadius(userId, 30, It.IsAny<DateTime>()));
        }

        [TestMethod]
        public void should_not_set_sight_radius_lower_than_the_minimum_sight_radius_for_user()
        {
            Guid userId = Guid.NewGuid();

            Guid itemId = Guid.NewGuid();

            //given a user that exists
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(true);

            //and an item that exists with a negative sight radius
            Mock<IItem> mockItem = new Mock<IItem>();
            mockItem.SetupGet(s => s.Distance).Returns(-15);
            mockItem.SetupGet(s => s.IsOneTimeUse).Returns(true);

            _itemRetriever.Setup(s => s.ItemExists(itemId)).Returns(true);

            _itemRetriever.Setup(s => s.GetItemById(itemId)).Returns(mockItem.Object);

            //and a user sight radius
            _userSightRadiusProvider.Setup(s => s.GetUserSightRadius(userId, It.IsAny<DateTime>())).Returns(10);

            //and a min sight radius
            _userSightRadiusProvider.Setup(s => s.GetUserMinSightRadius(userId)).Returns(10);

            //should update user's sight radius with min value, because usage of item will cause sight radius to go below minimum
            _itemUsageDirector.UseItem(userId, itemId);

            _userSaver.Verify(s => s.SaveLastSightRadius(userId, 10, It.IsAny<DateTime>()));
        }
    }
}
