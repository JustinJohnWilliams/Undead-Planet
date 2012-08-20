using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserPotentialProvider
    {
        /// <summary>
        /// Returns the highest possible energy a user can have given their current inventory and cash.
        /// </summary>
        int GetMaxPotentialEnergy(Guid userId);

        /// <summary>
        /// Returns the highest possible attack power a user can have given their current inventory and cash.
        /// </summary>
        int GetMaxPotentialAttackPower(Guid userId);
    }
}
