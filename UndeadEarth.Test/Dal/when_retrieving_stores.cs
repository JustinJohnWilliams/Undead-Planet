using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Test.Dal.Utility;
using Moq;
using UndeadEarth.Dal;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_retrieving_stores
    {
        private string _connectionString;
        private IStoreRetriever _storeRetriever;
        private Mock<IDistanceCalculator> _distanceCalculator;

        private int _storeCount;

        private Guid _storeId1;
        private double _latitude1;
        private double _longitude1;
        private string _name1;

        private Guid _storeId2;
        private double _latitude2;
        private double _longitude2;
        private string _name2;

        private Guid _storeId3;
        private double _latitude3;
        private double _longitude3;
        private string _name3;

        private string _itemDescription1;
        private Guid _itemId1;
        private string _itemName1;
        private int _itemPrice1;
        private int _itemDistance1;
        private int _itemEnergy1;

        private int _itemCount;
        private Guid _hotZoneId;

        private double _radiusToUse;

        public when_retrieving_stores()
        {
            _distanceCalculator = new Mock<IDistanceCalculator>();
            _connectionString = DalTestContextSpecification.ConnectionString;
            _storeRetriever = new StoreRepository(_connectionString, _distanceCalculator.Object);

            _storeId1 = Guid.NewGuid();
            _latitude1 = 0;
            _longitude1 = 0;
            _name1 = "Store 1";

            _storeId2 = Guid.NewGuid();
            _latitude2 = 1;
            _longitude2 = 1;
            _name2 = "Store 2";

            _storeId3 = Guid.NewGuid();
            _latitude3 = 10;
            _longitude3 = 10;
            _name3 = "Store 3";

            _itemId1 = Guid.NewGuid();
            _itemDescription1 = "Item 1";
            _itemName1 = "Name1";
            _itemPrice1 = 500;
            _itemDistance1 = 0;
            _itemEnergy1 = 100;

            _radiusToUse = 5.0;
            _hotZoneId = Guid.NewGuid();
        }

        [TestInitialize()]
        public void BecauseOf()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                List<StoreDto> storeDtos = new List<StoreDto>();

                storeDtos.Add(new StoreDto
                {
                    Id = _storeId1,
                    Latitude = (decimal)_latitude1,
                    Longitude = (decimal)_longitude1,
                    Name = _name1,
                    HotZoneId = _hotZoneId
                });

                storeDtos.Add(new StoreDto
                {
                    Id = _storeId2,
                    Latitude = (decimal)_latitude2,
                    Longitude = (decimal)_longitude2,
                    Name = _name2,
                    HotZoneId = _hotZoneId
                });

                storeDtos.Add(new StoreDto
                {
                    Id = _storeId3,
                    Latitude = (decimal)_latitude3,
                    Longitude = (decimal)_longitude3,
                    Name = _name3,
                    HotZoneId = _hotZoneId
                });

                dataContext.StoreDtos.InsertAllOnSubmit(storeDtos);

                List<ItemDto> itemDtos = new List<ItemDto>();

                itemDtos.Add(new ItemDto
                {
                    Description = _itemDescription1,
                    Distance = _itemDistance1,
                    Energy = _itemEnergy1,
                    Id = _itemId1,
                    Name = _itemName1,
                    Price = _itemPrice1
                });

                dataContext.ItemDtos.InsertAllOnSubmit(itemDtos);

                dataContext.SubmitChanges();

                _storeCount = dataContext.StoreDtos.Count();
                _itemCount = dataContext.ItemDtos.Count();
            }
        }

        [TestCleanup()]
        public void CleanUp()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                dataContext.StoreDtos.DeleteAllOnSubmit(
                                dataContext.StoreDtos.Where(c => c.Id == _storeId1
                                                            || c.Id == _storeId2
                                                            || c.Id == _storeId3));
                dataContext.ItemDtos.DeleteAllOnSubmit(
                                dataContext.ItemDtos.Where(c => c.Id == _itemId1));

                dataContext.SubmitChanges();
            }
        }

        [TestMethod]
        public void should_retrieve_all_stores()
        {
            List<IStore> stores = _storeRetriever.GetAllStores();

            Assert.AreEqual(_storeCount, stores.Count());

            Assert.IsTrue(stores.Any(c => c.Id == _storeId1));
            IStore store1 = stores.SingleOrDefault(c => c.Id == _storeId1);

            Assert.AreEqual(_latitude1, store1.Latitude);
            Assert.AreEqual(_longitude1, store1.Longitude);
            Assert.AreEqual(_name1, store1.Name);

            Assert.IsTrue(stores.Any(c => c.Id == _storeId2));
            IStore store2 = stores.SingleOrDefault(c => c.Id == _storeId2);

            Assert.AreEqual(_latitude2, store2.Latitude);
            Assert.AreEqual(_longitude2, store2.Longitude);
            Assert.AreEqual(_name2, store2.Name);
        }

        [TestMethod]
        public void should_return_true_if_store_exists()
        {
            bool storeExists = _storeRetriever.StoreExists(_latitude1, _longitude1);
            Assert.IsTrue(storeExists);
        }

        [TestMethod]
        public void should_return_false_if_store_does_not_exist()
        {
            bool storeExists = _storeRetriever.StoreExists(-999, -999);
            Assert.IsFalse(storeExists);
        }

        [TestMethod]
        public void should_return_all_items_in_store()
        {
            List<IItem> itemsInStore = _storeRetriever.GetItems(_storeId1);

            Assert.AreEqual(_itemCount, itemsInStore.Count());

            Assert.IsTrue(itemsInStore.Any(c => c.Id == _itemId1));

            IItem item = itemsInStore.SingleOrDefault(c => c.Id == _itemId1);
            Assert.AreEqual(_itemId1, item.Id);
            Assert.AreEqual(_itemDescription1, item.Description);
            Assert.AreEqual(_itemDistance1, item.Distance);
            Assert.AreEqual(_itemEnergy1, item.Energy);
            Assert.AreEqual(_itemName1, item.Name);
            Assert.AreEqual(_itemDescription1, item.Description);
        }

        [TestMethod]
        public void should_return_all_stores_in_radius()
        {
            _distanceCalculator.Setup(
                            s => s.GetEasternPoint(_latitude1, _longitude1, _radiusToUse))
                            .Returns(new UndeadEarth.Contract.Tuple<double, double>(0, 3));

            _distanceCalculator.Setup(
                            s => s.GetNorthernPoint(_latitude1, _longitude1, _radiusToUse))
                            .Returns(new UndeadEarth.Contract.Tuple<double, double>(3, 0));

            _distanceCalculator.Setup(
                            s => s.GetWesternPoint(_latitude1, _longitude1, _radiusToUse))
                            .Returns(new UndeadEarth.Contract.Tuple<double, double>(0, -3));

            _distanceCalculator.Setup(
                            s => s.GetSouthernPoint(_latitude1, _longitude1, _radiusToUse))
                            .Returns(new UndeadEarth.Contract.Tuple<double, double>(-3, 0));

            _distanceCalculator.Setup(c => c.CalculateMiles(_latitude1, _longitude1, _latitude2, _longitude2)).Returns(10);

            List<IStore> stores = _storeRetriever.GetAllStoresInRadius(_latitude1, _longitude1, _radiusToUse)
                                                             .OrderBy(c => c.Latitude).Cast<IStore>().ToList();

            Assert.AreEqual(1, stores.Count());

            Assert.AreEqual(_storeId1, stores.ElementAt(0).Id);
            Assert.AreEqual(_latitude1, stores.ElementAt(0).Latitude);
            Assert.AreEqual(_longitude1, stores.ElementAt(0).Longitude);
        }

        [TestMethod]
        public void should_retrieve_stores_associated_with_hotzone()
        {
            List<IStore> stores = _storeRetriever.GetAllStoresByHotZoneId(_hotZoneId);
            Assert.AreEqual(3, stores.Count);
        }

        [TestMethod]
        public void should_retriever_an_empty_list_given_invalid_hotzone_id()
        {
            List<IStore> stores = _storeRetriever.GetAllStoresByHotZoneId(Guid.NewGuid());
            Assert.AreEqual(0, stores.Count);
        }
    }
}
