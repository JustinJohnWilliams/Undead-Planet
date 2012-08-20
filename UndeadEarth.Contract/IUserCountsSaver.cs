using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserCountsSaver
    {
        /// <summary>
        /// Adds miles tally for user.
        /// </summary>
        void AddMiles(Guid userId, int miles);

        /// <summary>
        /// Adds zombie tally for user.
        /// </summary>
        void AddZombiesKilled(Guid userId, int zombies);

        /// <summary>
        /// Adds zombies destroyed to zombie pack tally.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="zombiePacksDestroyed"></param>
        void AddZombiePacksDestroyed(Guid userId, int zombiePacksDestroyed);

        /// <summary>
        /// Add hotzone tally for user.
        /// </summary>
        void AddHotZonesDestroyed(Guid userId, int hotzoneCount);

        /// <summary>
        /// Records the peak attack power for user.
        /// </summary>
        void RecordPeakZombiesDestroyed(Guid userId, int attackPower);

        /// <summary>
        /// Add money tally for user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="money"></param>
        void AddMoney(Guid userId, int money);

        /// <summary>
        /// Inserts an empty count record for user if user counts doesn't exist.
        /// </summary>
        /// <param name="userId"></param>
        void InsertCounts(Guid userId);
    }
}
