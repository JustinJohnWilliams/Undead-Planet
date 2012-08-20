using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    /// <summary>
    /// Determines the difficult of a zombie pack.
    /// </summary>
    public interface IZombiePackDifficultyDirector
    {
        /// <summary>
        /// Determines the max zombie packs for a given zombie pack.  Take user's potential attack power into consideration along with
        /// if the user has moved to a different hotzone and came back (handles all of the rebalancing).
        /// </summary>
        IUserZombiePackProgress GetCalculatedZombiePackProgress(Guid userId, Guid zombiePackId);

        /// <summary>
        /// Determines the cost for hunting at a zombie pack.
        /// </summary>
        int GetEnergyCost(Guid userId, Guid zombiePackId);
    }
}
