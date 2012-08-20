using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Test.Dal.Utility;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_retrieving_items
    {
        private IItemRetriever _itemRetriever;
        private IUserItemRetriever _userItemRetriever;
        private ISafeHouseItemSaver _safeHouseItemSaver;
        private IUserItemSaver _userItemSaver;
        private string _connectionString;

        private Guid _itemId1;
        private string _description1;
        private int? _distance1;
        private int? _energy1;
        private string _name1;
        private int _price1;
        private int? _attack1;
        private bool _isOneTimeUse1;

        private Guid _itemId2;
        private string _description2;
        private int? _distance2;
        private int? _energy2;
        private string _name2;
        private int _price2;
        private int? _attack2;
        private bool _isOneTimeUse2;

        private Guid _userId1;
        private Guid _userId2;

        private Guid _userItemId1;
        private Guid _userItemId2;

        private Guid _itemId3;
        private Guid _userId3;
        private Guid _userItemId3;

        private Guid _itemId4;

        Guid _newSafeHouseId;
        Guid _newItemId;
        Guid _newUserId;

        Guid _safeHouseItemId;
        Guid _safeHouseItemId2;

        private Guid _safeHouseId1;

        public when_retrieving_items()
        {
            _itemRetriever = DalTestContextSpecification.Instance.Resolve<IItemRetriever>();
            _userItemRetriever = DalTestContextSpecification.Instance.Resolve<IUserItemRetriever>();
            _safeHouseItemSaver = DalTestContextSpecification.Instance.Resolve<ISafeHouseItemSaver>();
            _userItemSaver = DalTestContextSpecification.Instance.Resolve<IUserItemSaver>();

            _connectionString = DalTestContextSpecification.ConnectionString;

            _itemId1 = Guid.NewGuid();
            _description1 = "description 1";
            _distance1 = 5;
            _energy1 = 5;
            _name1 = "name 1";
            _price1 = 500;
            _attack1 = 10;
            _isOneTimeUse1 = true;

            _itemId2 = Guid.NewGuid();
            _description2 = "description 2";
            _distance2 = 10;
            _energy2 = 0;
            _name2 = "name 2";
            _price2 = 5000;
            _attack2 = null;
            _isOneTimeUse2 = false;

            _userId1 = Guid.NewGuid();
            _userId2 = Guid.NewGuid();

            _userItemId1 = Guid.NewGuid();
            _userItemId2 = Guid.NewGuid();

            _newItemId = Guid.NewGuid();
            _newSafeHouseId = Guid.NewGuid();
            _newUserId = Guid.NewGuid();

            _itemId3 = Guid.NewGuid();
            _userId3 = Guid.NewGuid();
            _userItemId3 = Guid.NewGuid();

            _itemId4 = Guid.NewGuid();

            _safeHouseItemId2 = Guid.NewGuid();

            _safeHouseId1 = Guid.NewGuid();
        }

        [TestInitialize()]
        public void BecauseOf()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                List<ItemDto> itemDtos = new List<ItemDto>();

                itemDtos.Add(new ItemDto
                {
                    Description = _description1,
                    Distance = _distance1,
                    Energy = _energy1,
                    Id = _itemId1,
                    Name = _name1,
                    Price = _price1,
                    Attack = _attack1,
                    IsOneTimeUse = _isOneTimeUse1
                });

                itemDtos.Add(new ItemDto
                {
                    Description = _description2,
                    Distance = _distance2,
                    Energy = _energy2,
                    Id = _itemId2,
                    Name = _name2,
                    Price = _price2,
                    Attack = _attack2,
                    IsOneTimeUse = _isOneTimeUse2
                });

                itemDtos.Add(new ItemDto
                {
                    Description = _description1,
                    Attack = null,
                    Distance = null,
                    Energy = null,
                    Id = _itemId4,
                    IsOneTimeUse = _isOneTimeUse1,
                    Name = _name1,
                    Price = _price1
                });

                dataContext.ItemDtos.InsertAllOnSubmit(itemDtos);
                dataContext.SubmitChanges();

                List<UserItemDto> userItemDtos = new List<UserItemDto>();

                userItemDtos.Add(new UserItemDto
                {
                    UserItemId = _userItemId1,
                    ItemId = _itemId1,
                    UserId = _userId2
                });

                userItemDtos.Add(new UserItemDto
                {
                    UserItemId = _userItemId2,
                    ItemId = _itemId2,
                    UserId = _userId2
                });

                userItemDtos.Add(new UserItemDto
                {
                    UserItemId = _userItemId3,
                    ItemId = _itemId3,
                    UserId = _userId3
                });

                dataContext.UserItemDtos.InsertAllOnSubmit(userItemDtos);
                dataContext.SubmitChanges();

                List<SafeHouseItemDto> safeHouseItemDtos = new List<SafeHouseItemDto>();
                safeHouseItemDtos.Add(new SafeHouseItemDto
                {
                    Id = _safeHouseItemId2,
                    ItemId = _itemId1,
                    SafeHouseId = _safeHouseId1,
                    UserId = _userId1
                });

                dataContext.SafeHouseItemDtos.InsertAllOnSubmit(safeHouseItemDtos);
                dataContext.SubmitChanges();
            }
        }

        [TestCleanup()]
        public void CleanUp()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                dataContext.ItemDtos.DeleteAllOnSubmit(
                                dataContext.ItemDtos.Where(c => c.Id == _itemId1
                                                        || c.Id == _itemId2
                                                        || c.Id == _itemId4));

                dataContext.SubmitChanges();

                dataContext.UserItemDtos.DeleteAllOnSubmit(
                                dataContext.UserItemDtos.Where(c => c.UserItemId == _userItemId1
                                                            || c.UserItemId == _userItemId2
                                                            || c.UserItemId == _userItemId3));

                dataContext.SubmitChanges();

                dataContext.SafeHouseItemDtos.DeleteAllOnSubmit(
                                dataContext.SafeHouseItemDtos.Where(c => c.Id == _safeHouseItemId
                                                                    || c.Id == _safeHouseItemId2));

                dataContext.SubmitChanges();
            }
        }

        [TestMethod]
        public void should_retrieve_item_by_item_id()
        {
            IItem item = _itemRetriever.GetItemById(_itemId1);

            Assert.AreEqual(_itemId1, item.Id);
            Assert.AreEqual(_name1, item.Name);
            Assert.AreEqual(_description1, item.Description);
            Assert.AreEqual(_energy1, item.Energy);
            Assert.AreEqual(_price1, item.Price);
            Assert.AreEqual(_distance1, item.Distance);
            Assert.AreEqual(_attack1, item.Attack);
            Assert.AreEqual(_isOneTimeUse1, item.IsOneTimeUse);
        }

        [TestMethod]
        public void should_return_null_if_no_item_found()
        {
            IItem item = _itemRetriever.GetItemById(Guid.NewGuid());

            Assert.IsNull(item);
        }

        [TestMethod]
        public void should_return_list_of_user_items_from_user_id()
        {
            List<IItem> items = _userItemRetriever.GetUserItems(_userId2)
                                                  .OrderBy(c => c.Description).Cast<IItem>().ToList();

            Assert.AreEqual(2, items.Count());

            Assert.AreEqual(_itemId1, items.ElementAt(0).Id);
            Assert.AreEqual(_name1, items.ElementAt(0).Name);
            Assert.AreEqual(_description1, items.ElementAt(0).Description);
            Assert.AreEqual(_energy1, items.ElementAt(0).Energy);
            Assert.AreEqual(_price1, items.ElementAt(0).Price);
            Assert.AreEqual(_distance1, items.ElementAt(0).Distance);
            Assert.AreEqual(_attack1, items.ElementAt(0).Attack);
            Assert.AreEqual(_isOneTimeUse1, items.ElementAt(0).IsOneTimeUse);

            Assert.AreEqual(_itemId2, items.ElementAt(1).Id);
            Assert.AreEqual(_name2, items.ElementAt(1).Name);
            Assert.AreEqual(_description2, items.ElementAt(1).Description);
            Assert.AreEqual(_energy2, items.ElementAt(1).Energy);
            Assert.AreEqual(_price2, items.ElementAt(1).Price);
            Assert.AreEqual(_distance2, items.ElementAt(1).Distance);
            Assert.AreEqual(0, items.ElementAt(1).Attack);
            Assert.AreEqual(_isOneTimeUse2, items.ElementAt(1).IsOneTimeUse);
        }

        [TestMethod]
        public void should_return_items_below_a_given_prices()
        {
            //no items should be returned 
            Assert.AreEqual(0, _itemRetriever.GetAllBelowPrice(0).Count);

            //make sure the itme that costs 500 is not in the list
            List<IItem> items = _itemRetriever.GetAllBelowPrice(499);
            Assert.IsFalse(items.Any(s => s.Id == _itemId1));

            //ensure item is in list
            items = _itemRetriever.GetAllBelowPrice(500);
            Assert.IsTrue(items.Any(s => s.Id == _itemId1));

            //ensure itme is in the list
            items = _itemRetriever.GetAllBelowPrice(5000);
            Assert.IsTrue(items.Any(s => s.Id == _itemId2));
        }

        [TestMethod]
        public void should_return_false_if_item_does_not_exist()
        {
            bool itemExists = _itemRetriever.ItemExists(Guid.NewGuid());

            Assert.IsFalse(itemExists);
        }

        [TestMethod]
        public void should_return_true_if_item_does_exist()
        {
            bool itemExists = _itemRetriever.ItemExists(_itemId1);

            Assert.IsTrue(itemExists);
        }

        [TestMethod]
        public void should_return_true_if_user_has_item()
        {
            bool userItemExists = _userItemRetriever.UserHasItem(_userId2, _itemId1);

            Assert.IsTrue(userItemExists);
        }

        [TestMethod]
        public void should_return_false_if_user_does_not_have_item()
        {
            bool userItemExists = _userItemRetriever.UserHasItem(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsFalse(userItemExists);
        }

        [TestMethod]
        public void should_save_user_item_in_safe_house()
        {

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                Assert.IsFalse(dataContext.SafeHouseItemDtos.Any(c => c.SafeHouseId == _newSafeHouseId
                                                                && c.ItemId == _newItemId
                                                                && c.UserId == _newUserId));
            }

            _safeHouseItemSaver.SaveItemInSafeHouse(_newSafeHouseId, _newItemId, _newUserId);

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                Assert.IsTrue(dataContext.SafeHouseItemDtos.Any(c => c.SafeHouseId == _newSafeHouseId
                                                                && c.ItemId == _newItemId
                                                                && c.UserId == _newUserId));

                _safeHouseItemId = dataContext.SafeHouseItemDtos.First(c => c.SafeHouseId == _newSafeHouseId
                                                                        && c.ItemId == _newItemId
                                                                        && c.UserId == _newUserId).Id;
            }
        }

        [TestMethod]
        public void should_get_user_item_id()
        {
            Guid userItemId = _userItemRetriever.GetUserItemId(_userId2, _itemId1);

            Assert.AreEqual(_userItemId1, userItemId);
        }

        [TestMethod]
        public void should_remove_user_item()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                Assert.IsTrue(dataContext.UserItemDtos.Any(c => c.UserItemId == _userItemId3));
            }

            _userItemSaver.RemoveUserItem(_userItemId3);

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                Assert.IsFalse(dataContext.UserItemDtos.Any(c => c.UserItemId == _userItemId3));
            }
        }

        [TestMethod]
        public void should_remove_safe_house_item()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                Assert.IsTrue(dataContext.SafeHouseItemDtos.Any(c => c.Id == _safeHouseItemId2));
            }

            _safeHouseItemSaver.RemoveSafeHouseItem(_safeHouseItemId2);

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                Assert.IsFalse(dataContext.SafeHouseItemDtos.Any(c => c.Id == _safeHouseItemId2));
            }
        }

        [TestMethod]
        public void should_get_user_item_count()
        {
            int userItemCount = _userItemRetriever.GetUserItemCount(_userId2);

            Assert.AreEqual(2, userItemCount);

            int userItemEmptyCount = _userItemRetriever.GetUserItemCount(Guid.NewGuid());

            Assert.AreEqual(0, userItemEmptyCount);
        }

        [TestMethod]
        public void should_return_zero_if_energy_is_null()
        {
            IItem item = _itemRetriever.GetItemById(_itemId4);

            Assert.AreEqual(0, item.Energy);
        }

        [TestMethod]
        public void should_return_zero_if_distance_is_null()
        {
            IItem item = _itemRetriever.GetItemById(_itemId4);

            Assert.AreEqual(0, item.Distance);
        }

        [TestMethod]
        public void should_return_zero_if_attack_is_null()
        {
            IItem item = _itemRetriever.GetItemById(_itemId4);

            Assert.AreEqual(0, item.Attack);
        }
    }
}