using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserLevelService
    {
        /// <summary>
        /// Checks user level and determines if the base attributes need to be increased base on zombie kills
        /// </summary>
        /// <param name="userId"></param>
        void CheckForLevelUp(Guid userId);

        /// <summary>
        /// Retrieves the number of zombies needed to be killed before next level increase
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        long GetZombieCountForLevelUp(int level);

        /// <summary>
        /// Retrieves base attack power for given level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        int GetBaseAttackPowerAttributeForUserGivenLevel(int level);

        /// <summary>
        /// Retrieves base energy for given level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        int GetBaseEnergyAttributeForUserGivenLevel(int level);

        /// <summary>
        /// Retrieves item slot for given level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        int GetBaseItemSlotAttributeForUserGivenLevel(int level);

        /// <summary>
        /// Retrieves user's current level
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetCurrentLevelForUser(Guid userId);
    }
}
