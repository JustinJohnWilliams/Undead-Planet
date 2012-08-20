using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class UserLevelService : IUserLevelService
    {
        private IUserRetriever _userRetriever;
        private IUserStatsRetriever _userStatsRetriever;
        private IUserSaver _userSaver;

        private double _magicInventorySlotNumber;
        private double _magicAttackPowerNumber;
        private double _magicEnergyNumber;
        private double _magicZombieNumber;

        /// <summary>
        /// Initializes a new instance of the UserLevelService class.
        /// </summary>
        public UserLevelService(IUserRetriever userRetriever, IUserStatsRetriever userStatsRetriever, IUserSaver userSaver)
        {
            _userRetriever = userRetriever;
            _userStatsRetriever = userStatsRetriever;
            _userSaver = userSaver;

            _magicInventorySlotNumber = 30;
            _magicAttackPowerNumber = .9;
            _magicEnergyNumber = 1086; 
            _magicZombieNumber = 2.95154499;
        }

        void IUserLevelService.CheckForLevelUp(Guid userId)
        {
            if (!_userRetriever.UserExists(userId))
            {
                return;
            }
            
            UserStats userStats = _userStatsRetriever.GetStats(userId);

            long zombiesKilled = userStats.ZombiesKilled;

            int userLevel = _userRetriever.GetCurrentLevel(userId);
           
            long zombiesForNextLevel = GetZombieKillsForNextLevel(userLevel);

            if (zombiesKilled >= zombiesForNextLevel)
            {
                int newLevel = userLevel + 1;

                _userSaver.SetUserLevel(userId, newLevel);

                int updatedUserEnergy = (this as IUserLevelService).GetBaseEnergyAttributeForUserGivenLevel(newLevel);//(int)Math.Abs(Math.Round(Math.Pow(2, (100 - newLevel) / _magicEnergyNumber) - 1000, 0)) + 99;
                _userSaver.UpdateCurrentBaseEnergy(userId, updatedUserEnergy);
                _userSaver.SaveLastEnergy(userId, updatedUserEnergy, DateTime.Now);

                int updatedUserAttackPower = (this as IUserLevelService).GetBaseAttackPowerAttributeForUserGivenLevel(newLevel);
                _userSaver.UpdateCurrentBaseAttack(userId, updatedUserAttackPower);

                int updatedInventoryAmount = (this as IUserLevelService).GetBaseItemSlotAttributeForUserGivenLevel(newLevel);
                _userSaver.UpdateUserInventorySlot(userId, updatedInventoryAmount);
            }
        }

        long IUserLevelService.GetZombieCountForLevelUp(int level)
        {
            return GetZombieKillsForNextLevel(level);
        }

        int IUserLevelService.GetBaseAttackPowerAttributeForUserGivenLevel(int level)
        {
            return (int)Math.Round(Math.Abs(Math.Log(level, _magicAttackPowerNumber)));
        }

        int IUserLevelService.GetBaseEnergyAttributeForUserGivenLevel(int level)
        {
            return (int)Math.Round(((_magicEnergyNumber / 100) * level) + 100);
        }

        int IUserLevelService.GetBaseItemSlotAttributeForUserGivenLevel(int level)
        {
            return (int)Math.Abs(Math.Round(Math.Pow(2, (100 - level) / _magicInventorySlotNumber) - 16, 0));
        }

        int IUserLevelService.GetCurrentLevelForUser(Guid userId)
        {
            if (_userRetriever.UserExists(userId) == false)
            {
                throw new InvalidOperationException("User does not exist");
            }

            return _userRetriever.GetCurrentLevel(userId);
        }

        private long GetZombieKillsForNextLevel(int userLevel)
        {
            return (long)Math.Round(20 * Math.Pow(userLevel, _magicZombieNumber), 0);
        }
    }
}
