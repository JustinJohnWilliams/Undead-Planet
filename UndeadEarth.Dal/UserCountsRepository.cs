using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using UndeadEarth.Dal.Database;

namespace UndeadEarth.Dal
{
    public class UserCountsRepository : IUserStatsRetriever, IUserCountsSaver
    {
        private string _connectionString;
        public UserCountsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region IUserCountsRetriever Implementation

        //long IUserCountsRetriever.GetZombieKillCountForUser(Guid userId)
        //{
        //    AddCountIfNeeded(userId);
        //    using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
        //    {
        //        dataContext.ReadUncommited();

        //        return GetUserCount(dataContext, userId).ZombiesKilled;
        //    }
        //}

        //long IUserCountsRetriever.GetMilesTraveledByUser(Guid userId)
        //{
        //    AddCountIfNeeded(userId);
        //    using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
        //    {
        //        dataContext.ReadUncommited();

        //        return GetUserCount(dataContext, userId).Miles;
        //    }
        //}

        //long IUserCountsRetriever.GetZombiePacksDestroyedForUser(Guid userId)
        //{
        //    AddCountIfNeeded(userId);
        //    using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
        //    {
        //        dataContext.ReadUncommited();

        //        return GetUserCount(dataContext, userId).ZombiePacksDestroyed;
        //    }
        //}

        //long IUserCountsRetriever.GetHotZonesDestroyedCount(Guid userId)
        //{
        //    AddCountIfNeeded(userId);
        //    using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
        //    {
        //        dataContext.ReadUncommited();

        //        return GetUserCount(dataContext, userId).HotZonesDestroyed;
        //    }
        //}

        //long IUserCountsRetriever.GetPeakZombiesKilled(Guid userId)
        //{
        //    AddCountIfNeeded(userId);
        //    using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
        //    {
        //        dataContext.ReadUncommited();

        //        return GetUserCount(dataContext, userId).PeakAttack;
        //    }
        //}

        //long IUserCountsRetriever.GetAccumulatedMoney(Guid userId)
        //{
        //    using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
        //    {
        //        dataContext.ReadUncommited();

        //        return GetUserCount(dataContext, userId).AccumulatedMoney;
        //    }
        //}

        List<UserStats> IUserStatsRetriever.GetUsersRank(List<long> facebookUserIds)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                var results = from counts in dataContext.UserCountsDtos
                              join user in dataContext.UserDtos
                              on counts.UserId equals user.Id
                              where facebookUserIds.Contains(user.FacebookUserId)
                              orderby user.Level descending,
                              counts.HotZonesDestroyed descending,
                              counts.ZombiePacksDestroyed descending,
                              counts.ZombiesKilled descending,
                              counts.AccumulatedMoney descending,
                              counts.Miles descending
                              select new UserStats
                              {
                                  HotZonesDestroyed = counts.HotZonesDestroyed,
                                  KillStreak = counts.PeakAttack,
                                  Level = user.Level,
                                  MilesTraveled = counts.Miles,
                                  MoneyAccumulated = counts.AccumulatedMoney,
                                  Name = user.DisplayName,
                                  ZombiePacksDestroyed = counts.ZombiePacksDestroyed,
                                  ZombiesKilled = counts.ZombiesKilled
                              };

                return results.ToList();
            }
        }


        #endregion

        #region IUserCountsSaver Implementation

        void IUserCountsSaver.AddMiles(Guid userId, int miles)
        {
            AddCountIfNeeded(userId);
            using (UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserCountDto userCountDto = GetUserCount(undeadEarthDataContext, userId);

                userCountDto.Miles += miles;
                undeadEarthDataContext.SubmitChanges();
            }
        }

        void IUserCountsSaver.AddZombiesKilled(Guid userId, int zombies)
        {
            AddCountIfNeeded(userId);
            using (UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserCountDto userCountDto = GetUserCount(undeadEarthDataContext, userId);

                userCountDto.ZombiesKilled += zombies;
                undeadEarthDataContext.SubmitChanges();
            }
        }

        void IUserCountsSaver.AddHotZonesDestroyed(Guid userId, int hotzoneCount)
        {
            AddCountIfNeeded(userId);
            using (UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserCountDto userCountDto = GetUserCount(undeadEarthDataContext, userId);

                userCountDto.HotZonesDestroyed += hotzoneCount;
                undeadEarthDataContext.SubmitChanges();
            }
        }

        void IUserCountsSaver.RecordPeakZombiesDestroyed(Guid userId, int attackPower)
        {
            AddCountIfNeeded(userId);
            using (UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserCountDto userCountDto = GetUserCount(undeadEarthDataContext, userId);

                if (userCountDto.PeakAttack < attackPower)
                {
                    userCountDto.PeakAttack = attackPower;
                    undeadEarthDataContext.SubmitChanges();
                }
            }
        }

        void IUserCountsSaver.AddZombiePacksDestroyed(Guid userId, int zombiePacksDestroyed)
        {
            AddCountIfNeeded(userId);
            using (UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserCountDto userCountDto = GetUserCount(undeadEarthDataContext, userId);

                userCountDto.ZombiePacksDestroyed += zombiePacksDestroyed;
                undeadEarthDataContext.SubmitChanges();
            }
        }

        void IUserCountsSaver.AddMoney(Guid userId, int money)
        {
            AddCountIfNeeded(userId);
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserCountDto userCountDto = GetUserCount(dataContext, userId);

                userCountDto.AccumulatedMoney += money;
                dataContext.SubmitChanges();
            }
        }

        void IUserCountsSaver.InsertCounts(Guid userId)
        {
            AddCountIfNeeded(userId);
        }

        #endregion

        #region Private Method

        private void AddCountIfNeeded(Guid userId)
        {
            using (UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(_connectionString))
            {
                undeadEarthDataContext.ReadUncommited();

                if (undeadEarthDataContext.UserCountsDtos.Any(u => u.UserId == userId) == false)
                {
                    UserCountDto userCountDto = new UserCountDto
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Miles = 0,
                        HotZonesDestroyed = 0,
                        PeakAttack = 0,
                        ZombiesKilled = 0
                    };

                    undeadEarthDataContext.UserCountsDtos.InsertOnSubmit(userCountDto);
                    undeadEarthDataContext.SubmitChanges();
                }
            }
        }

        private UserCountDto GetUserCount(UndeadEarthDataContext undeadEarthDataContext, Guid userId)
        {
            //TODO: when a user gets created, automatically place an empty count entry for user
            return undeadEarthDataContext.GetUserCountByUserId(userId);
        }

        #endregion

        UserStats IUserStatsRetriever.GetStats(Guid userId)
        {
            UserStats userStats;
            using (UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(_connectionString))
            {
                userStats = (from counts in undeadEarthDataContext.UserCountsDtos
                             join user in undeadEarthDataContext.UserDtos
                             on counts.UserId equals user.Id
                             where user.Id == userId
                             select new UserStats
                             {
                                 HotZonesDestroyed = counts.HotZonesDestroyed,
                                 KillStreak = counts.PeakAttack,
                                 Level = user.Level,
                                 MilesTraveled = counts.Miles,
                                 MoneyAccumulated = counts.AccumulatedMoney,
                                 Name = user.DisplayName,
                                 ZombiePacksDestroyed = counts.ZombiePacksDestroyed,
                                 ZombiesKilled = counts.ZombiesKilled
                             }).FirstOrDefault();

                userStats = userStats ?? new UserStats();
            }

            return userStats;
        }
    }
}
