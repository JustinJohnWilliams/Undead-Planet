using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Dal;
using UndeadEarth.Test.Dal.Utility;

namespace UndeadEarth.Test.Dal
{
    [TestClass]
    public class when_retrieving_user_rank
    {
        private IUserStatsRetriever _userStatsRetriever;
        private List<Guid> _userIds;
        public when_retrieving_user_rank()
        {
            _userStatsRetriever = new UserCountsRepository(DalTestContextSpecification.ConnectionString);
            _userIds = new List<Guid>();
        }

        [TestMethod]
        public void should_disregard_facebook_user_ids_that_dont_exist()
        {
            TheRankingForUsers(10, 11, 12, 13, 14).ShouldBeEmpty();
        }

        [TestMethod]
        public void should_return_just_1_userrank()
        {
            GivenUser(withFacebookId: 10, level: 5, name: "Jane", killed: 1, miles: 2, packs: 3, hotzones: 4, money: 5, streak: 6);


            UserStats userRank = TheRankingForUsers(10).First();
            userRank.HotZonesDestroyed.ShouldBe(4L);
            userRank.KillStreak.ShouldBe(6L);
            userRank.Level.ShouldBe(5);
            userRank.MilesTraveled.ShouldBe(2L);
            userRank.MoneyAccumulated.ShouldBe(5L);
            userRank.Name.ShouldBe("Jane");
            userRank.ZombiePacksDestroyed.ShouldBe(3L);
            userRank.ZombiesKilled.ShouldBe(1L);
        }

        [TestMethod]
        public void should_return_2_user_ranks_ordered()
        {
            GivenUser(withFacebookId: 10, level: 5, name: "Jane", killed: 1, miles: 2, packs: 3, hotzones: 4, money: 5, streak: 6);
            GivenUser(withFacebookId: 11, level: 7, name: "John", killed: 8, miles: 9, packs: 10, hotzones: 11, money: 12, streak: 13);


            UserStats userRank = TheRankingForUsers(10, 11).First();
            userRank.HotZonesDestroyed.ShouldBe(11L);
            userRank.KillStreak.ShouldBe(13L);
            userRank.Level.ShouldBe(7);
            userRank.MilesTraveled.ShouldBe(9L);
            userRank.MoneyAccumulated.ShouldBe(12L);
            userRank.Name.ShouldBe("John");
            userRank.ZombiePacksDestroyed.ShouldBe(10L);
            userRank.ZombiesKilled.ShouldBe(8L);

            userRank = TheRankingForUsers(10, 11).Last();
            userRank.HotZonesDestroyed.ShouldBe(4L);
            userRank.KillStreak.ShouldBe(6L);
            userRank.Level.ShouldBe(5);
            userRank.MilesTraveled.ShouldBe(2L);
            userRank.MoneyAccumulated.ShouldBe(5L);
            userRank.Name.ShouldBe("Jane");
            userRank.ZombiePacksDestroyed.ShouldBe(3L);
            userRank.ZombiesKilled.ShouldBe(1L);
        }

        [TestMethod]
        public void should_return_rank_based_on_level()
        {
            GivenUser(withFacebookId: 10, level: 1, name: "A");
            GivenUser(withFacebookId: 11, level: 2, name: "B");
            GivenUser(withFacebookId: 12, level: 3, name: "C");

            TheRankingForUsers(10, 11, 12).First().Name.ShouldBe("C");
            TheRankingForUsers(10, 11, 12).Last().Name.ShouldBe("A");
        }

        private List<UserStats> TheRankingForUsers(params long[] args)
        {
            return _userStatsRetriever.GetUsersRank(args.ToList());
        }

        private void GivenUser(int withFacebookId, int level, string name, long killed = 0, long miles = 0, long packs = 0, long hotzones = 0, long money = 0, long streak = 0)
        {
            TestDataContext testDataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            Guid userId = Guid.NewGuid();
            UserDto userDto = new UserDto
            {
                Id = userId,
                BaseLineAttackPower = 0,
                BaseLineEnergy = 0,
                CurrentBaseAttack = 0,
                CurrentBaseEnergy = 0,
                DisplayName = name,
                Email = string.Empty,
                FacebookUserId = withFacebookId,
                Latitude = 0,
                Level = level,
                ZoneId = Guid.Empty
            };

            testDataContext.UserDtos.InsertOnSubmit(userDto);
            testDataContext.SubmitChanges();

            UserCountDto userCountDto = new UserCountDto
            {
                AccumulatedMoney = money,
                HotZonesDestroyed = hotzones,
                Id = Guid.NewGuid(),
                Miles = miles,
                PeakAttack = streak,
                UserId = userId,
                ZombiePacksDestroyed = packs,
                ZombiesKilled = killed
            };

            testDataContext.UserCountDtos.InsertOnSubmit(userCountDto);
            testDataContext.SubmitChanges();

            _userIds.Add(userId);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestDataContext testDataContext = new TestDataContext(DalTestContextSpecification.ConnectionString);
            testDataContext.UserCountDtos.DeleteAllOnSubmit(testDataContext.UserCountDtos.Where(s => _userIds.Contains(s.UserId.Value)));
            testDataContext.SubmitChanges();

            testDataContext.UserDtos.DeleteAllOnSubmit(testDataContext.UserDtos.Where(s => _userIds.Contains(s.Id)));
            testDataContext.SubmitChanges();
        }
    }

    public static class extensions
    {
        public static void ShouldBeEmpty(this List<UserStats> userRanks)
        {
            Assert.AreEqual(0, userRanks.Count);
        }

        public static void ShouldBe(this object actual, object expected)
        {
            Assert.AreEqual(actual, expected);
        }
    }
}
