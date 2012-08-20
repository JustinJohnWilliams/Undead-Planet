using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserAttackPowerProvider
    {
        /// <summary>
        /// Gets the attack power of a given user.  Returns 0 if the user doesn't exist.  Returns the summation of all the 
        /// items attack power that the user has.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetAttackPower(Guid userId);
    }
}
