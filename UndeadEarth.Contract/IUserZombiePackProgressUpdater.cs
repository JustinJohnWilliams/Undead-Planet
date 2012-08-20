using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserZombiePackProgressSaver
    {
        /// <summary>
        /// Updates database to reflect a successful hunt for a user at specified zombie pack
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="zombiePackId"></param>
        void SaveZombiePackProgress(Guid userId, Guid zombiePackId, IUserZombiePackProgress progress);
    }
}
