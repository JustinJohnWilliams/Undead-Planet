using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserZombiePackProgressRetriever
    {
        /// <summary>
        /// Returns UserZombiePackProgress by user id and hot zone id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hotZoneId"></param>
        /// <returns></returns>
        IUserZombiePackProgress GetUserZombiePackProgressFor(Guid userId, Guid zombiePackId);

        /// <summary>
        /// Checks to see if current zombie pack is destroyed by user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="zombiePackId"></param>
        /// <returns></returns>
        bool IsZombiePackDestroyed(Guid userId, Guid zombiePackId);
    }
}
