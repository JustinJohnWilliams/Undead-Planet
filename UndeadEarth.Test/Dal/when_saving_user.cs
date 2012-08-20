using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using System.Configuration;
using UndeadEarth.Test.Dal.Utility;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_saving_user
    {
        private IUserSaver _userSaver;
        private string _connectionString;
        private Guid _idToUse;

        private Guid _currentHotZone;
        private Guid _newHotZoneIdToUse;

        private double _newHotZoneLatitude;
        private double _newHotZoneLongitude;

        public when_saving_user()
        {
            _userSaver = DalTestContextSpecification.Instance.Resolve<IUserSaver>();
            _connectionString = DalTestContextSpecification.ConnectionString;

            _idToUse = Guid.NewGuid();

            _currentHotZone = Guid.NewGuid();
            _newHotZoneIdToUse = Guid.NewGuid();

            _newHotZoneLatitude = 2.5;
            _newHotZoneLongitude = 2.5;
        }

        [TestInitialize()]
        public void BecauseOf()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto userDto = new UserDto
                {
                    DisplayName = "TestUserName",
                    Id = _idToUse,
                    Latitude = 1.0M,
                    Longitude = 1.0M,
                    Email = "Test@test.com",
                    ZoneId = _currentHotZone,
                    Money = 500,
                    Level = 1,
                    BaseLineEnergy = 15,
                    BaseLineAttackPower = 15,
                    PossibleItemAmount = 5
                };

                dataContext.UserDtos.InsertOnSubmit(userDto);
                dataContext.SubmitChanges();

                HotZoneDto hotZoneDto = new HotZoneDto
                {
                    CanStartHere = false,
                    Id = _newHotZoneIdToUse,
                    Latitude = (decimal)_newHotZoneLatitude,
                    Longitude = (decimal)_newHotZoneLongitude,
                    Name = "New Test HotZone"
                };

                dataContext.HotZoneDtos.InsertOnSubmit(hotZoneDto);
                dataContext.SubmitChanges();
            }
        }

        [TestCleanup()]
        public void CleanUp()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                dataContext.UserDtos.DeleteAllOnSubmit(
                                dataContext.UserDtos.Where(c => c.Id == _idToUse));

                dataContext.HotZoneDtos.DeleteAllOnSubmit(
                                dataContext.HotZoneDtos.Where(c => c.Id == _newHotZoneIdToUse));

                dataContext.SubmitChanges();
            }
        }

        [TestMethod]
        public void should_update_user_location_if_user_exists()
        {
            _userSaver.UpdateUserLocation(_idToUse, 2.0, 2.0);

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto user = dataContext.UserDtos.SingleOrDefault(c => c.Id == _idToUse);

                Assert.AreEqual(_idToUse, user.Id);
                Assert.AreEqual((decimal)2.0, user.Latitude);
                Assert.AreEqual((decimal)2.0, user.Longitude);
            }
        }

        [TestMethod]
        public void should_update_hot_zone_if_user_is_on_new_hot_zone()
        {
            _userSaver.UpdateUserLocation(_idToUse, _newHotZoneLatitude, _newHotZoneLongitude);

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto userDto = dataContext.UserDtos.First(c => c.Id == _idToUse);

                Assert.AreEqual(_newHotZoneIdToUse, userDto.ZoneId);
            }
        }

        [TestMethod]
        public void should_not_throw_error_if_user_does_not_exist()
        {
            Guid newId = Guid.NewGuid();

            _userSaver.UpdateUserLocation(newId, 3.0, 3.0);

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                Assert.IsNull(dataContext.UserDtos.SingleOrDefault(c => c.Id == newId));
            }
        }

        [TestMethod]
        public void should_update_zone_user_is_in()
        {
            _userSaver.UpdateZone(_idToUse, _newHotZoneIdToUse);

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto userDto = dataContext.UserDtos.SingleOrDefault(c => c.Id == _idToUse);

                Assert.AreEqual(_newHotZoneIdToUse, userDto.ZoneId);
            }
        }

        [TestMethod]
        public void should_add_money_passed_in_to_money_of_the_current_user()
        {
            //users by default start off with 500 money (db constraint).
            _userSaver.AddMoney(_idToUse, 100);

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto userDto = dataContext.UserDtos.SingleOrDefault(c => c.Id == _idToUse);

                Assert.AreEqual(600, userDto.Money);
            }
        }

        [TestMethod]
        public void should_update_last_visited_hotzone_for_user()
        {
            Guid hotZoneId = Guid.NewGuid();
            _userSaver.UpdateLastVisitedHotZone(_idToUse, hotZoneId);

            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto userDto = dataContext.UserDtos.SingleOrDefault(c => c.Id == _idToUse);
                Assert.AreEqual(userDto.LastVisitedHotZoneId, hotZoneId);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_exception_if_update_on_last_visit_hotzone_is_done_on_user_that_doest_exist()
        {
            _userSaver.UpdateLastVisitedHotZone(Guid.NewGuid(), Guid.NewGuid());
        }

        [TestMethod]
        public void should_update_baseline_attack_power_for_user()
        {
            _userSaver.UpdateBaseLine(_idToUse, 1000, 1200);
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto userDto = dataContext.UserDtos.SingleOrDefault(c => c.Id == _idToUse);
                Assert.AreEqual(userDto.BaseLineAttackPower, 1000);
                Assert.AreEqual(userDto.BaseLineEnergy, 1200);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_exception_if_update_on_baseline_is_done_on_user_that_doest_exist()
        {
            _userSaver.UpdateBaseLine(Guid.NewGuid(), 0, 0);
        }

        [TestMethod]
        public void should_set_user_level()
        {
            _userSaver.SetUserLevel(_idToUse, 2);
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto userDto = dataContext.UserDtos.SingleOrDefault(c => c.Id == _idToUse);
                Assert.AreEqual(2, userDto.Level);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void should_throw_error_if_trying_update_user_level_and_user_does_not_exist()
        {
            _userSaver.SetUserLevel(Guid.NewGuid(), 1);
        }

        [TestMethod]
        public void should_update_users_base_energy()
        {
            _userSaver.UpdateEnergyForDifficultyCalculation(_idToUse, 50);
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto userDto = dataContext.UserDtos.SingleOrDefault(c => c.Id == _idToUse);
                Assert.AreEqual(50, userDto.BaseLineEnergy);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void should_throw_error_if_trying_to_update_base_line_energy_and_user_does_not_exist()
        {
            _userSaver.UpdateEnergyForDifficultyCalculation(Guid.NewGuid(), 50);
        }

        [TestMethod]
        public void should_update_users_base_attack()
        {
            _userSaver.UpdateAttackForDifficultyCalculation(_idToUse, 50);
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto userDto = dataContext.UserDtos.SingleOrDefault(c => c.Id == _idToUse);
                Assert.AreEqual(50, userDto.BaseLineAttackPower);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void should_throw_error_if_trying_to_update_user_attack_power_and_user_does_not_exist()
        {
            _userSaver.UpdateAttackForDifficultyCalculation(Guid.NewGuid(), 50);
        }

        [TestMethod]
        public void should_update_users_inventory_amount()
        {
            _userSaver.UpdateUserInventorySlot(_idToUse, 10);
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto userDto = dataContext.UserDtos.SingleOrDefault(c => c.Id == _idToUse);
                Assert.AreEqual(10, userDto.PossibleItemAmount);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void should_throw_error_if_trying_to_update_user_inventory_amount_and_user_does_not_exist()
        {
            _userSaver.UpdateUserInventorySlot(Guid.NewGuid(), 50);
        }

        [TestMethod]
        public void should_set_user_current_base_attack()
        {
            _userSaver.UpdateCurrentBaseAttack(_idToUse, 500);
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto userDto = dataContext.UserDtos.SingleOrDefault(c => c.Id == _idToUse);
                Assert.AreEqual(500, userDto.CurrentBaseAttack);
            }
        }

        [TestMethod]
        public void should_set_user_current_base_energy()
        {
            _userSaver.UpdateCurrentBaseEnergy(_idToUse, 20);
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                UserDto userDto = dataContext.UserDtos.SingleOrDefault(c => c.Id == _idToUse);
                Assert.AreEqual(20, userDto.CurrentBaseEnergy);
            }
        }
    }
}
