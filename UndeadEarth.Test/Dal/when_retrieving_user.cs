using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Test.Dal.Utility;
using System.Configuration;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_retrieving_user
    {
        private string _connectionString;

        private IUserRetriever _userRetriever;

        private string _displayName;
        private string _email;
        private Guid _idToUse;
        private double _lattitude;
        private Guid _locationIdToUse;
        private double _longitude;
        private Guid _zoneIdToUse;
        private Guid _lastVisitedHotZoneId;
        private int _money;
        private int _possibleItemAmount1;

        private string _displayName2;
        private string _email2;
        private Guid _idToUse2;
        private double _lattitude2;
        private Guid _locationIdToUse2;
        private double _longitude2;
        private Guid _zoneIdToUse2;
        private int _money2;
        private int _possibleItemAmount2;

        public when_retrieving_user()
        {
            _connectionString = DalTestContextSpecification.ConnectionString;

            _userRetriever = DalTestContextSpecification.Instance.Resolve<IUserRetriever>();

            _displayName = "DisplayName";
            _email = "Test@test.com";
            _idToUse = Guid.NewGuid();
            _lattitude = 1234.5678;
            _locationIdToUse = Guid.NewGuid();
            _longitude = 5678.1234;
            _zoneIdToUse = Guid.NewGuid();
            _lastVisitedHotZoneId = Guid.NewGuid();
            _money = 100;
            _possibleItemAmount1 = 5;

            _displayName2 = "DisplayName2";
            _email2 = "Test2@test.com";
            _idToUse2 = Guid.NewGuid();
            _lattitude2 = 1111.9999;
            _locationIdToUse2 = Guid.NewGuid();
            _longitude2 = 9999.1111;
            _zoneIdToUse2 = Guid.NewGuid();
            _money2 = 1000;
            _possibleItemAmount2 = 10;

        }

        [TestInitialize()]
        public void BeacauseOf()
        {
            using (TestDataContext dataConext = new TestDataContext(_connectionString))
            {
                List<UserDto> userDtos = new List<UserDto>();
                userDtos.Add(new UserDto
                            {
                                DisplayName = _displayName,
                                Email = _email,
                                Id = _idToUse,
                                Latitude = (decimal)_lattitude,
                                LocationId = _locationIdToUse,
                                Longitude = (decimal)_longitude,
                                ZoneId = _zoneIdToUse,
                                Money = _money,
                                LastVisitedHotZoneId = _lastVisitedHotZoneId,
                                BaseLineAttackPower = 1,
                                BaseLineEnergy = 100,
                                Level = 5,
                                PossibleItemAmount = _possibleItemAmount1,
                                CurrentBaseAttack = 1,
                                CurrentBaseEnergy = 100,
                                FacebookUserId = 200
                            });

                userDtos.Add(new UserDto
                            {
                                DisplayName = _displayName2,
                                Email = _email2,
                                Id = _idToUse2,
                                Latitude = (decimal)_lattitude2,
                                LocationId = _locationIdToUse2,
                                Longitude = (decimal)_longitude2,
                                ZoneId = _zoneIdToUse2,
                                BaseLineAttackPower = 1,
                                BaseLineEnergy = 100,
                                Money = _money2,
                                PossibleItemAmount = _possibleItemAmount2
                            });

                dataConext.UserDtos.InsertAllOnSubmit(userDtos);
                dataConext.SubmitChanges();
                        
            }
        }

        [TestCleanup()]
        public void CleanUp()
        {
            using (TestDataContext dataContext = new TestDataContext(_connectionString))
            {
                dataContext.UserDtos.DeleteAllOnSubmit(
                            dataContext.UserDtos.Where(c => c.Id == _idToUse
                                                        || c.Id == _idToUse2));

                dataContext.SubmitChanges();
            }
        }

        [TestMethod]
        public void should_return_user_if_user_exists()
        {
            IUser user = _userRetriever.GetUserById(_idToUse);

            Assert.AreEqual(_displayName, user.DisplayName);
            Assert.AreEqual(_email, user.Email);
            Assert.AreEqual(_idToUse, user.Id);
            Assert.AreEqual(_longitude, user.Longitude);
            Assert.AreEqual(_lattitude, user.Latitude);
            Assert.AreEqual(_locationIdToUse, user.LocationId);
            Assert.AreEqual(_zoneIdToUse, user.ZoneId);
            Assert.AreEqual(_money, user.Money);
            Assert.AreEqual(_possibleItemAmount1, user.PossibleItemAmount);
        }

        [TestMethod]
        public void should_return_null_if_user_does_not_exist()
        {
            IUser user = _userRetriever.GetUserById(Guid.NewGuid());

            Assert.IsNull(user);
        }

        [TestMethod]
        public void should_return_true_if_user_exists()
        {
            Assert.IsTrue(_userRetriever.UserExists(_idToUse));
        }

        [TestMethod]
        public void should_return_false_if_user_does_not_exist()
        {
            Assert.IsFalse(_userRetriever.UserExists(Guid.NewGuid()));
        }

        [TestMethod]
        public void should_return_the_amount_of_money_the_user_currently_has()
        {
            int money = _userRetriever.GetCurrentMoney(_idToUse);
            Assert.AreEqual(_money, money);

        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_error_when_returning_money_if_user_does_not_exist()
        {
            int money = _userRetriever.GetCurrentMoney(Guid.NewGuid());
        }

        [TestMethod]
        public void should_return_null_for_last_visited_hotzone_if_no_hotzone_has_been_set()
        {
            Guid? lastVisitedHotZone = _userRetriever.GetLastVisitedHotZone(_idToUse2);
            Assert.IsNull(lastVisitedHotZone);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_error_when_returning_last_visited_hotzone_if_user_doesnt_exist()
        {
            _userRetriever.GetLastVisitedHotZone(Guid.NewGuid());
        }

        [TestMethod]
        public void should_return_lastvisite_hotzone_id_for_user_that_has_visited_a_hotzone()
        {
            Guid? lastVisitedHotZone = _userRetriever.GetLastVisitedHotZone(_idToUse);
            Assert.AreEqual(lastVisitedHotZone, _lastVisitedHotZoneId);
        }

        [TestMethod]
        public void should_return_baseline_attack_power_for_user_that_exists()
        {
            Assert.AreEqual(1, _userRetriever.GetAttackPowerForDifficultyCalculation(_idToUse));
            Assert.AreEqual(1, _userRetriever.GetAttackPowerForDifficultyCalculation(_idToUse2));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_exception_when_returning_baseline_attack_power_for_user_that_doesnt_exist()
        {
            _userRetriever.GetAttackPowerForDifficultyCalculation(Guid.NewGuid());
        }

        [TestMethod]
        public void should_return_baseline_energy_for_user_that_exists()
        {
            Assert.AreEqual(100, _userRetriever.GetEnergyForDifficultyCalculation(_idToUse));
            Assert.AreEqual(100, _userRetriever.GetEnergyForDifficultyCalculation(_idToUse2));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_exception_when_returning_baseline_energy_for_user_that_doesnt_exist()
        {
            _userRetriever.GetEnergyForDifficultyCalculation(Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_exception_when_retrieving_user_level_and_user_does_not_exist()
        {
            int level = _userRetriever.GetCurrentLevel(Guid.NewGuid());
        }

        [TestMethod]
        public void should_retrieve_users_current_level()
        {
            int level = _userRetriever.GetCurrentLevel(_idToUse);
            Assert.AreEqual(5, level);
        }

        [TestMethod]
        public void should_retrieve_user_current_item_slot_capacity()
        {
            int itemSlotAmount = _userRetriever.GetCurrentBaseSlots(_idToUse);
            Assert.AreEqual(_possibleItemAmount1, itemSlotAmount);
        }

        [TestMethod]
        public void should_retrieve_user_current_base_attack()
        {
            int currentBaseAttack = _userRetriever.GetCurrentBaseAttackPower(_idToUse);
            Assert.AreEqual(1, currentBaseAttack);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_error_when_returning_user_current_base_attack_if_user_doesnt_exist()
        {
            _userRetriever.GetCurrentBaseAttackPower(Guid.NewGuid());
        }

        [TestMethod]
        public void should_retrieve_user_current_base_energy()
        {
            int currentBaseEnergy = _userRetriever.GetCurrentBaseEnergy(_idToUse);
            Assert.AreEqual(100, currentBaseEnergy);
        }

        [TestMethod]
        public void should_throw_error_when_returning_user_energy_and_user_doesnt_exist()
        {
            _userRetriever.GetCurrentBaseEnergy(_idToUse);
        }

        [TestMethod]
        public void should_return_user_by_facebook_user_id()
        {
            _userRetriever.GetUserByFacebookId(200);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void should_throw_error_if_facebook_user_id_is_0_and_retrieval_is_attempted()
        {
            _userRetriever.GetUserByFacebookId(0);
        }

        [TestMethod]
        public void should_return_null_for_user_that_doesnt_exist()
        {
            Assert.IsNull(_userRetriever.GetUserByFacebookId(-100));
        }

        [TestMethod]
        public void should_return_true_if_facebook_user_exists()
        {
            Assert.IsTrue(_userRetriever.FacebookUserExists(200));
        }

        [TestMethod]
        public void should_return_false_if_facebook_user_doesnt_exist()
        {
            Assert.IsFalse(_userRetriever.FacebookUserExists(-110));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void should_throw_error_if_facebook_user_id_is_0_user_exists_is_attempted()
        {
            _userRetriever.FacebookUserExists(0);
        }
    }
}
