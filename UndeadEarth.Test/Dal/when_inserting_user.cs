using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Dal;
using UndeadEarth.Test.Dal.Utility;
using UndeadEarth.Test;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_inserting_user
    {
        private IUserSaver _userSaver;
        private long _facebookUserId;

        [TestInitialize]
        public void TestInitialize()
        {
            _userSaver = new UserRepository(DalTestContextSpecification.ConnectionString);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestDataContext testDataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            if (testDataContext.UserDtos.Any(s => s.FacebookUserId == _facebookUserId))
            {
                testDataContext.UserDtos.DeleteOnSubmit(testDataContext.UserDtos.Single(s => s.FacebookUserId == _facebookUserId));
                testDataContext.SubmitChanges();
            }
        }

        [TestMethod]
        public void the_starting_energy_should_be_100()
        {
            TheNewUser(withBaseEnergy: 100).CurrentBaseEnergy.ShouldBe(100);
        }

        [TestMethod]
        public void the_energy_for_determining_zombie_pack_difficulty_should_be_100()
        {
            TheNewUser(withBaseEnergy: 100).BaseLineEnergy.ShouldBe(100);
        }

        [TestMethod]
        public void the_attack_for_determinging_zombie_pack_difficulty_should_be_1()
        {
            TheNewUser(withBaseAttack: 1).BaseLineAttackPower.ShouldBe(1);
        }

        [TestMethod]
        public void the_starting_location_should_be_saved()
        {
            Guid hotZoneId = Guid.NewGuid();
            TheNewUser(withStartingHotZoneId: hotZoneId).ZoneId.ShouldBe(hotZoneId);
        }

        [TestMethod]
        public void the_last_visited_hotzone_should_be_starting_location()
        {
            Guid hotZoneId = Guid.NewGuid();
            TheNewUser(withStartingHotZoneId: hotZoneId).LastVisitedHotZoneId.ShouldBe(hotZoneId);
        }

        [TestMethod]
        public void the_starting_attack_power_should_be_1()
        {
            TheNewUser(withBaseAttack: 1).BaseLineAttackPower.ShouldBe(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void should_throw_error_if_facebook_user_id_is_zero()
        {
            TheNewUser(withFacebookUserId: 0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void should_throw_error_if_user_already_exists()
        {
            GivenUserWithFacebookId(100);
            TheNewUser(withFacebookUserId: 100);
        }


        private UserDto TheNewUser(long withFacebookUserId = -100, string withName = "name", Guid withStartingHotZoneId = default(Guid), int withBaseAttack = 0, int withBaseEnergy = 0)
        {
            return TheNewUser(Guid.NewGuid(), withFacebookUserId, withName, withStartingHotZoneId, withBaseAttack, withBaseEnergy);
        }

        private UserDto TheNewUser(Guid withUserId, long withFacebookId = -100, string withName = "name", Guid withStartingHotZoneId = default(Guid), int withBaseAttack = 0, int withBaseEnergy = 0)
        {
            _facebookUserId = withFacebookId;
            _userSaver.InsertUser(withUserId, withFacebookId, withName, withStartingHotZoneId, withBaseAttack, withBaseEnergy);
            TestDataContext testDataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            return testDataContext.UserDtos.Single(s => s.Id == withUserId);
        }

        private void GivenUserWithFacebookId(long facebookUserId)
        {
            TestDataContext testDataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            testDataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            UserDto userDto = new UserDto
            {
                BaseLineAttackPower = 0,
                BaseLineEnergy = 0,
                PossibleItemAmount = 0,
                CurrentBaseAttack = 0,
                CurrentBaseEnergy = 0,
                DisplayName = string.Empty,
                Email = string.Empty,
                FacebookUserId = facebookUserId,
                Id = Guid.NewGuid(),
                Latitude = 0,
                ZoneId = Guid.Empty
            };

            testDataContext.UserDtos.InsertOnSubmit(userDto);

            testDataContext.SubmitChanges();
        }
    }
}
