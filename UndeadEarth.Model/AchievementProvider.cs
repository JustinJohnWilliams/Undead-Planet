using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class AchievementProvider : IAchievementProvider
    {
        private IUserStatsRetriever _userStatsRetriever;
        public AchievementProvider(IUserStatsRetriever userStatsRetriever)
        {
            _userStatsRetriever = userStatsRetriever;
        }

        List<string> IAchievementProvider.GetAchievementsForUser(Guid userId)
        {
            List<string> achievements = new List<string>();
            UserStats userStats = _userStatsRetriever.GetStats(userId);
            AddMileBasedAchievements(userId, achievements, userStats.MilesTraveled);
            AddZombieKillAchievements(userId, achievements, userStats.ZombiesKilled);
            AddZombiePackAchievements(userId, achievements, userStats.ZombiePacksDestroyed);
            AddHotzoneAchievements(userId, achievements, userStats.HotZonesDestroyed);
            AddPeakZombiesKilledAchievements(userId, achievements, userStats.KillStreak);
            AddMoneyAchievements(userId, achievements, userStats.MoneyAccumulated);
            return achievements;
        }

        private void AddZombiePackAchievements(Guid userId, List<string> achievements, long zombiePacksDestroyed)
        {
            ForCount(zombiePacksDestroyed, 1, "First Pack - Destroy 1 Zombie Pack.", achievements);
            ForCount(zombiePacksDestroyed, 10, "City Block - Destroy 10 Zombie Packs.", achievements);
            ForCount(zombiePacksDestroyed, 100, "Rampage - Destroy 100 Zombie Packs.", achievements);
            ForCount(zombiePacksDestroyed, 1000, "Get er Done - Destroy 1,000 Zombie Packs.", achievements);
            ForCount(zombiePacksDestroyed, 10000, "Madness - Destroy 10,000 Zombie Packs.", achievements);
        }

        private void AddMileBasedAchievements(Guid userId, List<string> achievements, long milesTraveled)
        {
            ForCount(milesTraveled, 10, "First Steps - Travel 10 miles.", achievements);
            ForCount(milesTraveled, 100, "Tourist - Travel 100 miles.", achievements);
            ForCount(milesTraveled, 1000, "Nomadic Hunter - Travel 1,000 miles", achievements);
            ForCount(milesTraveled, 10000, "Around the World - Travel 10,000 miles", achievements);
            ForCount(milesTraveled, 100000, "No Stone Unturned - Travel 100,000 miles", achievements);
        }

        private void AddZombieKillAchievements(Guid userId, List<string> achievements, long zombiesDestroyed)
        {
            ForCount(zombiesDestroyed, 1, "First Kill - Kill 1 zombie.", achievements);
            ForCount(zombiesDestroyed, 10, "First Ten - Kill 10 zombies.", achievements);
            ForCount(zombiesDestroyed, 100, "Hunter - Kill 100 zombies.", achievements);
            ForCount(zombiesDestroyed, 1000, "Genghis Khan - Kill 1,000 zombies.", achievements);
            ForCount(zombiesDestroyed, 10000, "Clearing House - Kill 10,000 zombies.", achievements);
            ForCount(zombiesDestroyed, 100000, "Killtacular - Kill 100,000 zombies.", achievements);
            ForCount(zombiesDestroyed, 1000000, "Genocide - Kill 1,000,000 zombies.", achievements);
        }

        private void AddHotzoneAchievements(Guid userId, List<string> achievements, long hotzone)
        {
            ForCount(hotzone, 1, "Apprentice Hunter - Destroy 1 Hot Zone.", achievements);
            ForCount(hotzone, 10, "Journey Man - Destroy 10 Hot Zones.", achievements);
            ForCount(hotzone, 100, "Master Hunter - Destroy 100 Hot Zones.", achievements);
        }

        private void AddPeakZombiesKilledAchievements(Guid userId, List<string> achievements, long peakZombies)
        {
            ForCount(peakZombies, 2, "Double Kill - Kill 2 zombies in one hunt.", achievements);
            ForCount(peakZombies, 10, "Multi Kill - Kill 10 zombies in one hunt.", achievements);
            ForCount(peakZombies, 100, "Power Overwhelming - Kill 100 zombies in one hunt.", achievements);
            ForCount(peakZombies, 1000, "God Like - Kill 1000 zombies in one hunt.", achievements);
        }

        private void AddMoneyAchievements(Guid userId, List<string> achievements, long money)
        {
            ForCount(money, 10, "Payday - Accumulate $10.", achievements);
            ForCount(money, 100, "Benjamin - Accumulate $100.", achievements);
            ForCount(money, 1000, "G's - Accumulate $1,000.", achievements);
            ForCount(money, 10000, "Movin on up - Accumulate $10,000.", achievements);
            ForCount(money, 100000, "Six Figs - Accumulate $100,000.", achievements);
            ForCount(money, 1000000, "Millionaire - Accumulate $1,000,000.", achievements);
        }

        void ForCount(long current, long target, string message, List<string> achievements)
        {
            if (current >= target)
            {
                achievements.Add(message);
            }
        }
    }
}
