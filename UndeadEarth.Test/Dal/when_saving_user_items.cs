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
    public class when_saving_user_items
    {
        private Guid _tentId;
        private IUserItemSaver _userItemSaver;
        private string _connectionString;

        private Guid _userItemId;

        private Guid _userId1;
        private Guid _userId2;

        private Guid _itemId1;
        private Guid _itemId2;

        public when_saving_user_items()
        {
            _userItemSaver = DalTestContextSpecification.Instance.Resolve<IUserItemSaver>();
            _connectionString = DalTestContextSpecification.ConnectionString;

            _userId1 = Guid.NewGuid();
            _userId2 = Guid.NewGuid();

            _itemId1 = Guid.NewGuid();
            _itemId2 = Guid.NewGuid();
        }

        [TestInitialize()]
        public void BecauseOf()
        {

        }

        [TestCleanup()]
        public void CleanUp()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                dataContext.UserItemDtos.DeleteAllOnSubmit(
                                dataContext.UserItemDtos.Where(c => c.UserItemId == _userItemId));

                dataContext.UserItemDtos.DeleteAllOnSubmit(
                                dataContext.UserItemDtos.Where(c => c.ItemId == _userItemId));

                dataContext.SubmitChanges();
            }
        }

        [TestMethod]
        public void should_save_item()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                Assert.IsFalse(dataContext.UserItemDtos.Any(c => c.ItemId == _itemId1 && c.UserId == _userId1));
            }

            _userItemSaver.SaveUserItem(_userId1, _itemId1);

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                Assert.IsTrue(dataContext.UserItemDtos.Any(c => c.ItemId == _itemId1 && c.UserId == _userId1));
                _userItemId = dataContext.UserItemDtos.SingleOrDefault(c => c.ItemId == _itemId1 && c.UserId == _userId1).UserItemId;
            }
        }

        [TestMethod]
        public void should_add_tent()
        {
            _tentId = new Guid("7CC21B96-B3AF-4257-B3B7-EA7113844654");

            _userItemSaver.AddTent(_userId1, 5);

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                Assert.AreEqual(5, dataContext.UserItemDtos.Count(s => s.ItemId == _tentId && s.UserId == _userId1));
            }
        }
    }
}
