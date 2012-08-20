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
    public class when_retrieving_safe_houses
    {
        private string _connectionString;
        private Mock<IDistanceCalculator> _distanceCalculator;

        private ISafeHouseRetriever _safeHouseRetriever;

        private Guid _safeHouseId1;
        private double _safeHouseLat1;
        private double _safeHouseLong1;
        
        private Guid _safeHouseId2;
        private double _safeHouseLat2;
        private double _safeHouseLong2;

        private Guid _safeHouseId3;
        private double _safeHouseLat3;
        private double _safeHouseLong3;

        private Guid _safeHouseItemId1;
        private Guid _userId1;
        private Guid _itemId1;

        private Guid _safeHouseItemId2;
        private Guid _userId2;
        private Guid _itemId2;

        private Guid _safeHouseItemId3;
        private Guid _userId3;
        private Guid _itemId3;

        private Guid _safehouseItemId4;

        private string _itemName1;
        private int? _itemEnergy1;
        private int? _itemDistance1;
        private int _itemPrice1;
        private string _itemDescription1;

        private string _itemName2;
        private int? _itemEnergy2;
        private int? _itemDistance2;
        private int _itemPrice2;
        private string _itemDescription2;

        private string _itemName3;
        private int? _itemEnergy3;
        private int? _itemDistance3;
        private int _itemPrice3;
        private string _itemDescription3;

        private Guid _hotZoneId;

        private double _radiusToUse;

        public when_retrieving_safe_houses()
        {
            _connectionString = DalTestContextSpecification.ConnectionString;

            _distanceCalculator = new Mock<IDistanceCalculator>();
            _safeHouseRetriever = new SafeHouseRepository(_connectionString, _distanceCalculator.Object);

            _safeHouseId1 = Guid.NewGuid();
            _safeHouseLat1 = 1.0;
            _safeHouseLong1 = 1.0;

            _safeHouseId2 = Guid.NewGuid();
            _safeHouseLat2 = 2.0;
            _safeHouseLong2 = 2.0;

            _safeHouseId3 = Guid.NewGuid();
            _safeHouseLat3 = 10.0;
            _safeHouseLong3 = 10.0;

            _safeHouseItemId1 = Guid.NewGuid();
            _itemId1 = Guid.NewGuid();
            _userId1 = Guid.NewGuid();

            _safeHouseItemId2 = Guid.NewGuid();
            _itemId1 = Guid.NewGuid();
            _userId2 = Guid.NewGuid();

            _safeHouseItemId3 = Guid.NewGuid();
            _itemId1 = Guid.NewGuid();
            _userId3 = Guid.NewGuid();

            _safehouseItemId4 = Guid.NewGuid();

            _itemName1 = "Item 1";
            _itemEnergy1 = 5;
            _itemDistance1 = 5;
            _itemDescription1 = "This is item 1";

            _itemName2 = "Item 2";
            _itemEnergy2 = 10;
            _itemDistance2 = 10;
            _itemDescription2 = "This is item 2";

            _itemName3 = "Item 3";
            _itemEnergy3 = null;
            _itemDistance3 = null;
            _itemDescription3 = "This is item 3";

            _radiusToUse = 5.0;

            _hotZoneId = Guid.NewGuid();
        }

        [TestInitialize()]
        public void BecauseOf()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                List<SafeHouseDto> safeHouseDtos = new List<SafeHouseDto>();
                safeHouseDtos.Add(new SafeHouseDto
                {
                    Id = _safeHouseId1,
                    Latitude = (decimal)_safeHouseLat1,
                    Longitude = (decimal)_safeHouseLong1,
                    HotZoneId = _hotZoneId
                });

                safeHouseDtos.Add(new SafeHouseDto
                {
                    Id = _safeHouseId2,
                    Latitude = (decimal)_safeHouseLat2,
                    Longitude = (decimal)_safeHouseLong2,
                    HotZoneId = _hotZoneId
                });

                safeHouseDtos.Add(new SafeHouseDto
                {
                    Id = _safeHouseId3,
                    Latitude = (decimal)_safeHouseLat3,
                    Longitude = (decimal)_safeHouseLong3,
                    HotZoneId = _hotZoneId
                });

                dataContext.SafeHouseDtos.InsertAllOnSubmit(safeHouseDtos);

                List<SafeHouseItemDto> safeHouseItemDtos = new List<SafeHouseItemDto>();
                safeHouseItemDtos.Add(new SafeHouseItemDto
                {
                    Id = _safeHouseItemId1,
                    ItemId = _itemId1,
                    SafeHouseId = _safeHouseId1,
                    UserId = _userId1
                });

                safeHouseItemDtos.Add(new SafeHouseItemDto
                {
                    Id = _safeHouseItemId2,
                    ItemId = _itemId2,
                    SafeHouseId = _safeHouseId1,
                    UserId = _userId1
                });

                safeHouseItemDtos.Add(new SafeHouseItemDto
                {
                    Id = _safeHouseItemId3,
                    ItemId = _itemId2,
                    SafeHouseId = _safeHouseId1,
                    UserId = _userId1
                });

                safeHouseItemDtos.Add(new SafeHouseItemDto
                {
                    Id = _safehouseItemId4,
                    ItemId = _itemId2,
                    SafeHouseId = _safeHouseId3,
                    UserId = _userId3
                });

                List<ItemDto> itemDtos = new List<ItemDto>();
                itemDtos.Add(new ItemDto
                {
                    Id = _itemId1,
                    Description = _itemDescription1,
                    Distance = _itemDistance1,
                    Energy = _itemEnergy1,
                    Name = _itemName1,
                    Price = _itemPrice1
                });
                
                itemDtos.Add(new ItemDto
                {
                    Id = _itemId2,
                    Description = _itemDescription2,
                    Distance = _itemDistance2,
                    Energy = _itemEnergy2,
                    Name = _itemName2,
                    Price = _itemPrice2
                });

                itemDtos.Add(new ItemDto
                {
                    Id = _itemId3,
                    Description = _itemDescription3,
                    Distance = _itemDistance3,
                    Energy = _itemEnergy3,
                    Name = _itemName3,
                    Price = _itemPrice3
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
                dataContext.SafeHouseDtos.DeleteAllOnSubmit(
                                        dataContext.SafeHouseDtos.Where(c => c.Id == _safeHouseId1
                                                                        || c.Id == _safeHouseId2
                                                                        || c.Id == _safeHouseId3));

                dataContext.SafeHouseItemDtos.DeleteAllOnSubmit(
                                        dataContext.SafeHouseItemDtos.Where(c => c.Id == _safeHouseItemId1
                                                                            || c.Id == _safeHouseItemId2
                                                                            || c.Id == _safeHouseItemId3
                                                                            || c.Id == _safehouseItemId4));

                dataContext.ItemDtos.DeleteAllOnSubmit(
                                        dataContext.ItemDtos.Where(c => c.Id == _itemId1
                                                                    || c.Id == _itemId2
                                                                    || c.Id == _itemId3));

                dataContext.SubmitChanges();
            }
        }

        [TestMethod]
        public void should_retrieve_all_safe_houses_within_radius()
        {
            _distanceCalculator.Setup(
                            s => s.GetEasternPoint(_safeHouseLat1, _safeHouseLong1, _radiusToUse))
                            .Returns(new UndeadEarth.Contract.Tuple<double, double>(0, 3));

            _distanceCalculator.Setup(
                            s => s.GetNorthernPoint(_safeHouseLat1, _safeHouseLong1, _radiusToUse))
                            .Returns(new UndeadEarth.Contract.Tuple<double, double>(3, 0));

            _distanceCalculator.Setup(
                            s => s.GetWesternPoint(_safeHouseLat1, _safeHouseLong1, _radiusToUse))
                            .Returns(new UndeadEarth.Contract.Tuple<double, double>(0, -3));

            _distanceCalculator.Setup(
                            s => s.GetSouthernPoint(_safeHouseLat1, _safeHouseLong1, _radiusToUse))
                            .Returns(new UndeadEarth.Contract.Tuple<double, double>(-3, 0));

            _distanceCalculator.Setup(c => c.CalculateMiles(_safeHouseLat1, _safeHouseLong1, _safeHouseLat2, _safeHouseLong2)).Returns(10);

            List<ISafeHouse> safeHouses = _safeHouseRetriever.GetAllSafeHousesInRadius(_safeHouseLat1, _safeHouseLong1, _radiusToUse)
                                                             .OrderBy(c => c.Latitude).Cast<ISafeHouse>().ToList();

            Assert.AreEqual(1, safeHouses.Count());

            Assert.AreEqual(_safeHouseId1, safeHouses.ElementAt(0).Id);
            Assert.AreEqual(_safeHouseLat1, safeHouses.ElementAt(0).Latitude);
            Assert.AreEqual(_safeHouseLong1, safeHouses.ElementAt(0).Longitude);
        }

        [TestMethod]
        public void should_return_true_if_safe_house_exists()
        {
            bool safeHouseExists = _safeHouseRetriever.SafeHouseExists(_safeHouseId1);

            Assert.IsTrue(safeHouseExists);
        }

        [TestMethod]
        public void should_return_false_if_safe_house_does_not_exist()
        {
            bool safeHouseExists = _safeHouseRetriever.SafeHouseExists(Guid.NewGuid());

            Assert.IsFalse(safeHouseExists);
        }

        [TestMethod]
        public void should_return_true_if_safe_house_has_item()
        {
            bool safeHouseHasItem = _safeHouseRetriever.SafeHouseHasItem(_itemId1, _userId1);

            Assert.IsTrue(safeHouseHasItem);
        }

        [TestMethod]
        public void should_return_false_if_safe_house_does_not_have_item()
        {
            bool safeHouseHasItem = _safeHouseRetriever.SafeHouseHasItem(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsFalse(safeHouseHasItem);

        }

        [TestMethod]
        public void should_retrieve_safe_house_item_id()
        {
            Guid safeHouseItemId = _safeHouseRetriever.GetSafeHouseItemId(_itemId1, _userId1);

            Assert.AreEqual(_safeHouseItemId1, safeHouseItemId);
        }

        [TestMethod]
        public void should_retrieve_list_of_items_in_safe_house_that_belong_to_user()
        {
            List<IItem> items = _safeHouseRetriever.GetItems(_userId1)
                                                   .OrderBy(c => c.Name).Cast<IItem>().ToList();
        }

        [TestMethod]
        public void should_retrieve_safe_houses_associated_with_hotzone()
        {
            List<ISafeHouse> safeHouses = _safeHouseRetriever.GetAllSafeHousesByHotZoneId(_hotZoneId);
            Assert.AreEqual(3, safeHouses.Count);
        }

        [TestMethod]
        public void should_retriever_an_empty_list_given_invalid_hotzone_id()
        {
            List<ISafeHouse> safeHouses = _safeHouseRetriever.GetAllSafeHousesByHotZoneId(Guid.NewGuid());
            Assert.AreEqual(0, safeHouses.Count);
        }
    }
}
